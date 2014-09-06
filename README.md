# ST4ImageDumper

This project is a fork of the discontinued ST4 Image Dumper. All credit goes to the original developer.  
You can DOWNLOAD this program here: [https://github.com/SuperGouge/ST4ImageDumper/releases](https://github.com/SuperGouge/ST4ImageDumper/releases)  
You can find the original official site here: [https://sites.google.com/site/st4imagedumper/](https://sites.google.com/site/st4imagedumper/)

## System Requirements:
* .NET Framework 3.5, 4.0 (full or client profile), or 4.5, or
* Linux/OS X/etc: Mono (included with Ubuntu but make sure libmono-winforms2.0-cil is installed)

## Usage
Fill in the settings (only folder and URL are required) and click Start. All
images in the specified folder will be posted. By default, a text file called
"_posted_.txt" will be created in the folder to keep track of which images have
been posted or skipped (e.g. board detects a duplicate image), but there is an
option to create a "Posted" subfolder and move the completed images there
instead.

If the 4chan Pass settings were not filled in, you must solve a captcha for
each post. You can enter the captchas in advance; they will be cached and used
as needed. The enter key is a shortcut for Submit and F5 is a shortcut for
Refresh. Captchas have a limited lifespan (5 minutes as of writing) so there's
no point in entering more than you will be able to use before they expire.
Captchas are validated upon input and discarded if incorrect. Unused captchas
are kept for subsequent dumps assuming they haven't expired.

You can drag a favicon from your browser onto the URL box to set the URL. You
can drag a folder from Explorer onto the Folder box to set the folder. You can
open the URL or folder that you set by double clicking on the label to the left
of its respective textbox.

Settings and captchas are saved across program runs. This data is stored in
your OS's application data folder in the "ST4 Image Dumper" subfolder.

## Wiki

For documentation, changelog and any other information, please visit the wiki: [https://github.com/SuperGouge/ST4ImageDumper/wiki](https://github.com/SuperGouge/ST4ImageDumper/wiki)

