Media Center Netflix Search Fix
===============================

This project is a Windows Service that addresses a problem with the Netflix plug-in for Windows Media Center (aka "MCE").  

Around June 2014, the "Search" feature of the Netflix plug-in stopped working, displaying either "Application Error" or 
"Service Unavailable" messages when trying to search.  All other Netflix streaming functionality still works fine.

The problem is due to a Microsoft-hosted web server that now responds only with a "504 Gateway Timeout" error.  Thankfully, 
the Windows Media Center Netflix plug-in does not rely on this defective server to actually perform the search - for that, 
it talks to the Netflix API directly.  However, in order to initialize the search, it requires a small bit of information 
(the "search taxonomy"), which it attempts to fetch from that Microsoft server. Since the server is no longer responding, the 
search cannot proceed.

This fix provides a locally-hosted substitute for Microsoft's broken server.  It is a Windows Service that listens for HTTP
requests at http://localhost:65123 (where 65123 is a configurable port number, specified in the MceNflx.exe.config file).

On startup, the service creates a "SearchTaxonomyPath" registry entry, specifying the localhost address, and overriding the 
default (broken) server URL from which Media Center normally fetches its seach taxonomy.
(The registry entry is under HKLM\Software\Microsoft\Windows\CurrentVersion\Media Center\Extension\InternetTV).

The service responds with the contents of "SearchTaxonomy.xml", which (to the best of my deductions) contains the bare minimum 
information necessary for Media Center to initialize its Netflix search. (Of course, none of this is documented publicly by 
Microsoft, but it seems to work!).
