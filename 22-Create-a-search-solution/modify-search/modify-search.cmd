@echo off

rem Set values for your Search service
set url=https://ai-search-00001.search.windows.net
set admin_key=BfMmDpzESwdBTBv8t00iQYKuckqGPrl7WEXdcuxPvuAzSeDYaiog

echo -----
echo Updating the skillset...
call curl -X PUT %url%/skillsets/margies-skillset?api-version=2020-06-30 -H "Content-Type: application/json" -H "api-key: %admin_key%" -d @skillset.json

rem wait
timeout /t 2 /nobreak

echo -----
echo Updating the index...
call curl -X PUT %url%/indexes/margies-index?api-version=2020-06-30 -H "Content-Type: application/json" -H "api-key: %admin_key%" -d @index.json

rem wait
timeout /t 2 /nobreak

echo -----
echo Updating the indexer...
call curl -X PUT %url%/indexers/mergies-indexer?api-version=2020-06-30 -H "Content-Type: application/json" -H "api-key: %admin_key%" -d @indexer.json

echo -----
echo Resetting the indexer
call curl -X POST %url%/indexers/mergies-indexer/reset?api-version=2020-06-30 -H "Content-Type: application/json" -H "Content-Length: 0" -H "api-key: %admin_key%" 

rem wait
timeout /t 5 /nobreak

echo -----
echo Rerunning the indexer
call curl -X POST %url%/indexers/mergies-indexer/run?api-version=2020-06-30 -H "Content-Type: application/json" -H "Content-Length: 0" -H "api-key: %admin_key%" 

