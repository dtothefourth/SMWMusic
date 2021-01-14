SMW Music Identifier by dtothefourth

This tool takes an SMW hack (.smc/.sfc) and will attempt to identify all of the custom music inserted
into the ROM and provide and soundtrack with links to the SMWC pages for each song.

All you have to do is click select ROM and choose the patched ROM for the hack and it will do the rest.

This will only work on ROMs which have music inserted using AddMusicK (AMK). Older insertions with AMM
will likely not be possible to identify currently.

The identification service relies on a web service with a database of information on all of the songs
hosted on SMWC which means:

	1) This tool requires an internet connection to run
	2) If for some reason the identification server is down the tool will not be able to run
	3) Only songs that have been added to the database will be available for identification

For 1/2 the program will provide an error message noting that the server could not be reached.

As for 3, every song on SMWC as of about 1/10/2021 is in the database and I will be updating it
occasionally or setting up an automated updated if there's sufficient demand.


The tool will attempt to match songs that have been updated or altered as well.
Songs that aren't a direct match will be noted as "(Modified Version?)" indicating that it
was not a perfect match and there's a possibility of a false positive.