@echo off

start "Unsubscribe Handler" Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe

start "DoesUserExist Handler"  Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe .\private$\sixeyed.messagequeue.doesuserexist
