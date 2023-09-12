# PalmTreeTime

Application for use on startup with windows to set a time frame for kids. It shutdowns if the time contingent is used.

Currently the following registry keys do have to exist in HKLM\SOFTWARE\electronic Ping [eP]\PalmTreeTime:

* "TimeContingent"=dword:00000005
* "LastTimeRun"="12/09/2000 20:53:22"

or use PalmTreeTime.reg for inital creation.

## Prerequsites
* NET 7.0 Runtime

## ToDos
* Calculate time when computer is actually running and not first started
* Special weekend time contingent (add to normal ones)
* Hide console window if everything checks. Show on time up; keep foreground till accepted to shutdown.
* Better visual for warning. So that parents walking nearby can easier spot the time up
* Better handling on null entries
* Nice icon
* Easy to use settings options