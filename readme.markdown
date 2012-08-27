# HTTP headers debugger

This is a simple GUI app for Windows (C#/Winforms) for inspecting HTTP headers.
I wrote it because I was having a problem that only seemed to manifest itself
on Chrome on Windows.

Simply provide it with some request headers and it'll make the request and 
print the verbatim headers it gets back. It will also summarise some of the 
more important bits into a more human readable format, if you're into that 
kind of thing.

License:  public domain, do what you want with it. There's nothing hard or clever
in here but patches welcome. 