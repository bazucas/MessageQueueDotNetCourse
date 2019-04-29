@echo off

start "Unsubscribe Handler" Sixeyed.MessageQueue.Handler.exe -listenOnQueueName:unsubscribe

start "DoesUserExist Handler" Sixeyed.MessageQueue.Handler.exe -listenOnQueueName:doesuserexist

start "Unsubscribe Legacy Handler" Sixeyed.MessageQueue.Handler.exe -listenOnQueueName:unsubscribe-legacy

start "Unsubscribe CRM Handler"  Sixeyed.MessageQueue.Handler.exe -listenOnQueueName:unsubscribe-crm

start "Unsubscribe Fulfilment Handler"  Sixeyed.MessageQueue.Handler.exe -listenOnQueueName:unsubscribe-fulfilment
