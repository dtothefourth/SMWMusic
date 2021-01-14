using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMWMusicGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;

            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }

        public void GetSongData(string rom)
        {

            Output.Text += "Getting Song list for " + Path.GetFileName(rom);
            Output.Text += "\r\n";

            String wd = Directory.GetCurrentDirectory();

            if (!File.Exists(rom)) return;

            bool hirom = IsHirom(rom);

            int slot = 1;
            MySqlConnection connection = new MySqlConnection("Server=dtothefourth.space,3306;Database=SMW_Music;User Id=blindkaizorace_db;Password=IRm6tQl.jZls;");
            try
            {
                connection.Open();
            } catch(Exception e)
            {
                MessageBox.Show("Could not connect to song database...");
                return;
            }
            try
            {
                BinaryReader creader = new BinaryReader(File.Open(rom, FileMode.Open, FileAccess.Read, FileShare.Read));
                uint co = 0, cdato = 0;
                co = SNESToPC(HasHeader(rom), false, 0xE8000);

                creader.BaseStream.Seek(co + 1, SeekOrigin.Begin);
                byte inb, inc;
                inb = creader.ReadByte();
                inc = creader.ReadByte();
                if (inb != 'A' || inc != 'M')
                {
                    creader.Close();
                    Output.Text += "\r\nThis ROM does not appear to have Addmusick applied.";
                    return;
                }

                creader.BaseStream.Seek(co + 8, SeekOrigin.Begin);
                co = creader.ReadUInt32();
                co &= 0x00FFFFFF;

                co = SNESToPC(HasHeader(rom), false, co);


                for (; ; slot++)
                {
                    Application.DoEvents();
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
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    
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
                            Output.AppendText("\r\n" + slot.ToString("X") + " - Originals - " + name);
                        }
                        else
                        {
                            Output.AppendText("\r\n" + slot.ToString("X") + " - https://smwc.me/s/" + ID + " - " + name);
                        }

                    }
                    else
                    {


                        dist = 0;
                        best = 0;

                        string l = data.Substring(0, 6);
                        string r = data.Substring(data.Length - 6, 6);

                        sql = "SELECT ID, Size, Data, URL, Name, Sub FROM Songs WHERE DataLeft = @param1 AND DataRight = @param2";
                        cmd = new MySqlCommand(sql, connection);

                        cmd.Parameters.AddWithValue("@param1", l);
                        cmd.Parameters.AddWithValue("@param2", r);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string compare = reader["Data"].ToString();

                            dist = (float)CalculateSimilarity(compare, data);

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

                        if (best >= 0.65)
                        {
                            if (ID == 0)
                            {
                                Output.AppendText("\r\n" + slot.ToString("X") + " - (Modified Version?) Originals - " + name);
                            }
                            else
                            {
                                Output.AppendText("\r\n" + slot.ToString("X") + " - (Modified Version?) https://smwc.me/s/" + ID + " - " + name);
                            }

                        }
                        else
                        {
                            Output.AppendText("\r\n" + slot.ToString("X") + " - Could not locate");
                        }



                        
                    }
                    
                }
                creader.Close();

            }


            catch (Exception e)
            {

            }
            connection.Close();
        }

        public bool HasHeader(string path)
        {
            if (!File.Exists(path)) return false;

            long size = new System.IO.FileInfo(path).Length;

            if ((size & 0x000200) != 0) return true;

            return false;
        }

        public bool IsHirom(string path)
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

        public uint SNESToPC(string path, uint addr)
        {
            return SNESToPC(path, addr & 0x0000FF, (addr & 0x00FF00) / 0x100, (addr & 0xFF0000) / 0x10000);
        }

        public uint SNESToPC(string path, uint addrlo, uint addrhi, uint bank)
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

        public uint SNESToPC(bool header, bool hirom, uint addr)
        {
            return SNESToPC(header, hirom, addr & 0x0000FF, (addr & 0x00FF00) / 0x100, (addr & 0xFF0000) / 0x10000);
        }

        public uint SNESToPC(bool header, bool hirom, uint addrlo, uint addrhi, uint bank)
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

        private void Select_ROM_Click(object sender, EventArgs e)
        {
            Select_ROM.Enabled = false;
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "SNES ROMs (*.smc;*.sfc)|*.smc;*.sfc",
                Title = "Open SMW ROM"
            };
            dialog.Multiselect = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Output.Text = "";
                GetSongData(dialog.FileName);
                Output.Text += "\r\n\r\nDone";
            }
            Select_ROM.Enabled = true;

        }

        private void Output_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }
    }
}
