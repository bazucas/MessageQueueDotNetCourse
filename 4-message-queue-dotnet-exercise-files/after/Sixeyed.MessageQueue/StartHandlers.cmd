@echo off

start "Unsubscribe Handler" Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe

start "DoesUserExist Handler"  Sixeyed.MessageQueue.Handler\bin\debug\Sixeyed.MessageQueue.Handler.exe .\private$\sixeyed.messagequeue.doesuserexist

start "Unsubscribe Legacy Handler"  Sixeyed.MessageQueue.Handlers.UnsubscribeLegacy\bin\debug\Sixeyed.MessageQueue.Handlers.UnsubscribeLegacy.exe

start "Unsubscribe CRM Handler"  Sixeyed.MessageQueue.Handlers.UnsubscribeCrm\bin\debug\Sixeyed.MessageQueue.Handlers.UnsubscribeCrm.exe

start "Unsubscribe Fulfilment Handler"  Sixeyed.MessageQueue.Handlers.UnsubscribeFulfilment\bin\debug\Sixeyed.MessageQueue.Handlers.UnsubscribeFulfilment.exe
