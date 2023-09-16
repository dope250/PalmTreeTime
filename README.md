# PalmTreeTime

Application for use on startup with windows to set a time frame for kids. It shutdowns if the time contingent is used. Bonus contingent is used on weekends.

Currently the following registry keys do have to exist in HKLM\SOFTWARE\electronic Ping [eP]\PalmTreeTime:

* "TimeContingent"=dword:00000005
* "LastTimeRun"="01/01/2000 01:00:00"
* "BonusContingent"=dword:00000005

or use PalmTreeTime.reg for inital creation.

## Prerequsites
* NET 7.0 Runtime

## ToDos
* Calculate time when computer is actually running and not first started
* ~~Special weekend time contingent (add to normal ones)~~
* ~~Hide console window if everything checks. Show on time up; keep foreground till accepted to shutdown.~~
* Detailed description on how to setup the task scheduler
* Better visual for warning. So that parents walking nearby can easier spot when time is up
* Better handling on null entries
* ~~Nice icon~~
* Easy to use settings page