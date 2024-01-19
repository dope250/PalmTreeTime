# PalmTreeTime

Application for use on startup with windows to set a time frame for kids. It shows a warning and shutdowns the computer if the time contingent is used up. Bonus contingent is used on weekends.

Currently the following registry keys do have to exist in HKLM\SOFTWARE\electronic Ping [eP]\PalmTreeTime:

* "TimeContingent"=dword:00000005
* "LastTimeRun"="01/01/2000 01:00:00"
* "BonusContingent"=dword:00000005
* "Tick"=dword:00000000

or use *PalmTreeTime.reg* for inital creation.

Change the following keys to your liking:

*TimeContingent* 
In minutes. Predefines the available time before the application nukes

*BonusContingent* 
In minutes. Bonus time which adds up to the normal TimeContingent. Is only used when the application is running on a weekend (Saturday & Sunday). Can also be kept to 0.

Any other keys should not be touched.


## Prerequsites
* NET 7.0 Desktop Runtime
* PalmTreeTime.reg registry keys already in place

## ToDos
* ~~Calculate time when computer is actually running and not first started~~
* ~~Simplify that point above. Just do an tick vs. timeContingent check holy fck.~~
* ~~Special weekend time contingent (add to normal ones)~~
* ~~Proper GUI and not a console window: Way to easy to manipulate the application behaviour~~
* ~~Hide console window if everything checks. Show on time up; keep foreground till accepted to shutdown.~~
* Detailed description on how to setup the task scheduler
* Better visual for warning. So that parents walking nearby can easier spot when time is up
* Better handling on null entries
* ~~Nice icon~~
* Easy to use settings page