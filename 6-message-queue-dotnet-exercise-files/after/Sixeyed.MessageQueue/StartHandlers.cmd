@echo off

start "Unsubscribe Handler" Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe unsubscribe

start "DoesUserExist Handler"  Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe doesuserexist

start "Unsubscribe Legacy Handler"  Sixeyed.MessageQueue.Handler.Unsubscribe\bin\debug\Sixeyed.MessageQueue.Handler.Unsubscribe.exe unsubscribe-legacy

start "Unsubscribe CRM Handler"  Sixeyed.MessageQueue.Handler.Unsubscribe\bin\debug\Sixeyed.MessageQueue.Handler.Unsubscribe.exe unsubscribe-crm

start "Unsubscribe Fulfilment Handler"  Sixeyed.MessageQueue.Handler.Unsubscribe\bin\debug\Sixeyed.MessageQueue.Handler.Unsubscribe.exe unsubscribe-fulfilment
