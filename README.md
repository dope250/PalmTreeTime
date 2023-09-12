# PalmTreeTime

Application for use on startup with windows to set a time frame for kids. It shutdowns if the time contingent is used.

Currently the following registry keys do have to exist in HKLM\SOFTWARE\electronic Ping [eP]\PalmTreeTime:

* "TimeContingent"=dword:00000005
* "LastTimeRun"="12/09/2000 20:53:22"

or use PalmTreeTime.reg for inital creation.

## Prerequsites
* NET 7.0 Runtime