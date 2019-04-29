@echo off

Sixeyed.MessageQueue.Handler.exe uninstall -instance:doesuserexist
Sixeyed.MessageQueue.Handler.exe uninstall -instance:unsubscribe
Sixeyed.MessageQueue.Handler.exe uninstall -instance:unsubscribe-crm
Sixeyed.MessageQueue.Handler.exe uninstall -instance:unsubscribe-fulfilment
Sixeyed.MessageQueue.Handler.exe uninstall -instance:unsubscribe-legacy

rmdir /s/q c:\handlers

md c:\handlers\doesuserexist
copy *.* c:\handlers\doesuserexist
c:\handlers\doesuserexist\Sixeyed.MessageQueue.Handler.exe install -username:.\Administrator -password:St0uffer -instance:doesuserexist

md c:\handlers\unsubscribe
copy *.* c:\handlers\unsubscribe
c:\handlers\unsubscribe\Sixeyed.MessageQueue.Handler.exe install -username:.\Administrator -password:St0uffer -instance:unsubscribe

md c:\handlers\unsubscribe-crm
copy *.* c:\handlers\unsubscribe-crm
c:\handlers\unsubscribe-crm\Sixeyed.MessageQueue.Handler.exe install -username:.\Administrator -password:St0uffer -instance:unsubscribe-crm

md c:\handlers\unsubscribe-fulfilment
copy *.* c:\handlers\unsubscribe-fulfilment
c:\handlers\unsubscribe-fulfilment\Sixeyed.MessageQueue.Handler.exe install -username:.\Administrator -password:St0uffer -instance:unsubscribe-fulfilment

md c:\handlers\unsubscribe-legacy
copy *.* c:\handlers\unsubscribe-legacy
c:\handlers\unsubscribe-legacy\Sixeyed.MessageQueue.Handler.exe install -username:.\Administrator -password:St0uffer -instance:unsubscribe-legacy
