@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

rem Set values for your storage account
set subscription_id=44ec6692-568c-4aa2-894d-f00a2b5930fc
set azure_storage_account=mystorageaccount000002
set azure_storage_key=eEXMHwiDZ3DpG5jbPJ359QzjUD6X4qt/YU/45axOWs59ihpxYKt9B+euvkgEn5V6FKcXHF4Ba6yf+AStf7uFBw==


echo Creating container...
call az storage container create --account-name !azure_storage_account! --subscription !subscription_id! --name margies --public-access blob --auth-mode key --account-key !azure_storage_key! --output none

echo Uploading files...
call az storage blob upload-batch -d margies -s data --account-name !azure_storage_account! --auth-mode key --account-key !azure_storage_key!  --output none
