using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace SMWMusic
{
    class Program
    {
        public class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }

        public static void CopyDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                if (!Directory.Exists(folders.Target))
                    Directory.CreateDirectory(folders.Target);
                foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
                {
                    File.Copy(file, Path.Combine(folders.Target, Path.GetFileName(file)), true);
                }

                foreach (var folder in Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
                }
            }
        }
        public static void GetSongData(string rom)
        {

            Console.WriteLine("Getting Song list for " + Path.GetFileName(rom));
            Console.WriteLine("");

            String wd = Directory.GetCurrentDirectory();

            if (!File.Exists(rom)) return;

            bool hirom = IsHirom(rom);

            int slot = 1;
            MySqlConnection connection = new MySqlConnection("Server=192.168.1.5,3306;Database=SMW_Music;User Id=blindkaizorace_db;Password=IRm6tQl.jZls;");
            connection.Open();
            try
            { 
                BinaryReader creader = new BinaryReader(File.Open(rom, FileMode.Open, FileAccess.Read, FileShare.Read));
                uint co = 0, cdato = 0;
                co = SNESToPC(HasHeader(rom), false, 0xE8000);

                creader.BaseStream.Seek(co + 8, SeekOrigin.Begin);
                co = creader.ReadUInt32();
                co &= 0x00FFFFFF;

                co = SNESToPC(HasHeader(rom), false, co);

                StreamWriter text = new StreamWriter("results.html",true);
                //text.WriteLine("<tr><td colspan=3 align=center>Song list for " + Path.GetFileName(rom) + "</td></tr>");

                for (; ; slot++)
                {
                    creader.BaseStream.Seek(co + 3 * slot, SeekOrigin.Begin);
                    cdato = creader.ReadUInt32();
                    cdato &= 0x00FFFFFF;

                    if (cdato == 0) continue;
                    if (cdato == 0xFFFFFF || slot == 0x100) break;


                    if (hirom && (cdato & 0x800000) != 0)
                        cdato |= 0x400000;
                    cdato = SNESToPC(HasHeader(rom), false, cdato);

                    creader.BaseStream.Seek(cdato, SeekOrigin.Begin);
                    ushort csize = creader.ReadUInt16();

                    ushort size = csize;
                    string data = "";
                    string item = "";
                    string name = "";
                    string URL = "";
                    uint ID = 0;

                    for (ushort i = 0; i < 24; i++)
                    {
                        creader.ReadByte();
                        csize--;
                    }

                    for (ushort i = 0; i < csize; i++)
                    {
                        byte b, cb;

                        cb = creader.ReadByte();

                        item = cb.ToString("X");
                        if (item.Length < 2) item = "0" + item;
                        data += item;
                    }

                    

                    
                    float dist = 0;
                    float best = 0;

                    
                    string sql = "SELECT ID, Size, Data, URL, Name, Sub FROM Songs WHERE Size = @param2";
                    using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@param2", size);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string compare = reader["Data"].ToString();

                            for (int i = 0; i < compare.Length; i++)
                            {
                                if (compare[i] == data[i]) dist++;
                            }

                            dist /= csize;
                            if (dist > best)
                            {
                                best = dist;

                                ID = uint.Parse(reader["ID"].ToString());
                                URL = reader["URL"].ToString();
                                name = reader["Name"].ToString();
                                name = name.Replace("&#039;", "'");
                            }

                        }

                        reader.Close();

                        if (best >= 1.4)
                        {
                            if (ID == 0)
                            {
                                Console.WriteLine(slot.ToString("X") + "  " + ID + "  " + name);
                                name = name.Replace("'", "&quot;");
                                text.WriteLine("<tr><td>" + slot.ToString("X") + " - </td><td>Originals - " + name + "</td><td></td></tr>");
                            }
                            else
                            {
                                Console.WriteLine(slot.ToString("X") + "  " + name);
                                name = name.Replace("'", "&quot;");
                                text.WriteLine("<tr><td>" + slot.ToString("X") + " - </td><td><a href='https://www.smwcentral.net/?p=section&a=details&id=" + ID + "'>" + name + "</a></td><td><a href='" + URL + "'>Download</a></td></tr>");
                            }
                        }
                        else
                        {
                            Console.WriteLine(slot.ToString("X") + "  Could not locate");
                            text.WriteLine("<tr><td>" + slot.ToString("X") + " - </td><td>Could not locate</td><td></td></tr>");
                        }
                    }
                }
                creader.Close();

                text.WriteLine("</table></body></html>");

                text.Close();
            }


            catch (Exception e)
            {

            }
            connection.Close();
        }

        public static bool HasHeader(string path)
        {
            if (!File.Exists(path)) return false;

            long size = new System.IO.FileInfo(path).Length;

            if ((size & 0x000200) != 0) return true;

            return false;
        }

        public static bool IsHirom(string path)
        {
            if (!File.Exists(path)) return false;
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));

            try
            {
                int o = 0x7FD5;
                if (HasHeader(path)) o += 0x200;
                reader.BaseStream.Seek(o, SeekOrigin.Begin);
                byte v = reader.ReadByte();
                reader.Close();
                if ((v & 1) == 1) return true;
                return false;
            }
            catch (Exception e)
            {
                reader.Close();
                return false;
            }
        }

        public static uint SNESToPC(string path, uint addr)
        {
            return SNESToPC(path, addr & 0x0000FF, (addr & 0x00FF00) / 0x100, (addr & 0xFF0000) / 0x10000);
        }

        public static uint SNESToPC(string path, uint addrlo, uint addrhi, uint bank)
        {

            bool header = HasHeader(path);

            if (IsHirom(path))
            {
                uint addr = (addrlo) + (addrhi * 0x100) + (bank * 0x10000);
                if ((addr & 0x400000) != 0)
                {
                    addr &= 0x3FFFFF;
                }
                else
                {
                    addr = ((addr & 0x8000) != 0) ? addr & 0x3FFFFF : 0;
                }
                return addr;
            }
            else
            {
                bank &= 0x7F;
                return (uint)((addrlo & 0xFF) + (0x100 * (addrhi & 0xFF)) + (0x8000 * bank) - ((header) ? 0 : 0x200) - 0x7E00);
            }
        }

        public static uint SNESToPC(bool header, bool hirom, uint addr)
        {
            return SNESToPC(header, hirom, addr & 0x0000FF, (addr & 0x00FF00) / 0x100, (addr & 0xFF0000) / 0x10000);
        }

        public static uint SNESToPC(bool header, bool hirom, uint addrlo, uint addrhi, uint bank)
        {

            if (hirom)
            {
                uint addr = (addrlo) + (addrhi * 0x100) + (bank * 0x10000);
                if ((addr & 0x400000) != 0)
                {
                    addr &= 0x3FFFFF;
                }
                else
                {
                    addr = ((addr & 0x8000) != 0) ? addr & 0x3FFFFF : 0;
                }
                return addr;
            }
            else
            {
                bank &= 0x7F;
                return (uint)((addrlo & 0xFF) + (0x100 * (addrhi & 0xFF)) + (0x8000 * bank) - ((header) ? 0 : 0x200) - 0x7E00);
            }
        }
        
        public static void UpdateNames()
        {
            MySqlConnection connection = new MySqlConnection("Server=192.168.1.5,3306;Database=SMW_Music;User Id=blindkaizorace_db;Password=IRm6tQl.jZls;");
            connection.Open();

            MySqlConnection wconnection = new MySqlConnection("Server=192.168.1.5,3306;Database=SMW_Music;User Id=blindkaizorace_db;Password=IRm6tQl.jZls;");
            wconnection.Open();


            WebClient x = new WebClient();

            string sql = "SELECT ID FROM Songs WHERE Sub = 0 AND Updated = 0";
            using (MySqlCommand cmd = new MySqlCommand(sql, connection))
            {
                
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    uint ID =  uint.Parse(reader["ID"].ToString());

                    try
                    {
                        string source = x.DownloadString("https://www.smwcentral.net/?p=section&a=details&id=" + ID);

                        string title = Regex.Match(source, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                        if(title != null && title.Length > 0)
                        {
                            title = title.Replace(" - SMW Music - SMW Central", "");
                            title = title.Replace("&#039;", "'");
                            title = title.Replace("Ã©", "e");
                            title = title.Replace("&amp;", "&");

                            MySqlCommand cmd2 = new MySqlCommand("UPDATE Songs SET Name = @param1, Updated = 1 WHERE ID = @param2", wconnection);

                            cmd2.Parameters.AddWithValue("@param1", title);
                            cmd2.Parameters.AddWithValue("@param2", ID);


                            cmd2.ExecuteNonQuery();
                        }

                        System.Threading.Thread.Sleep(250);

                    } catch (Exception e)
                    {

                    }
                }

                reader.Close();
            }

              
            connection.Close();
            wconnection.Close();
        }


        static void Main(string[] args)
        {
            //String wd = Directory.GetCurrentDirectory();
            String cd = AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(cd);

            //UpdateNames();
            //GetSongData("C:\\Users\\Justin\\Desktop\\SMWC\\HYPERION.v1.2.smc");
            GetSongData("ROM.smc");
            /*try
            {
                StreamWriter text = new StreamWriter("results.html", true);
                text.WriteLine(args[0]);
                text.Close();

                GetSongData(args[0]);
            } catch(Exception e)
            {

            }*/
        }
    }
}
