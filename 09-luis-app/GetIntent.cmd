@REM @echo off

@REM rem Set values for your Language Understanding app
@REM set app_id=YOUR_APP_ID
@REM set endpoint=YOUR_ENDPOINT
@REM set key=YOUR_KEY

@REM rem Get parameter and encode spaces for URL
@REM set input=%1
@REM set query=%input: =+%

@REM rem Use cURL to call the REST API
@REM curl -X GET "%endpoint%/luis/prediction/v3.0/apps/%app_id%/slots/production/predict?subscription-key=%key%&log=true&query=%query%"


curl -X POST "https://ai-language-svc-001.cognitiveservices.azure.com/language/:analyze-conversations?api-version=2022-10-01-preview" -H "Ocp-Apim-Subscription-Key: 712470ac96094a6193b617297ed33621"  -H "Apim-Request-Id: 4ffcac1c-b2fc-48ba-bd6d-b69d9942995a" -H "Content-Type: application/json" -d "{\"kind\":\"Conversation\",\"analysisInput\":{\"conversationItem\":{\"id\":\"1\",\"text\":\"What date will it be in New Delhi on Wednesday?\",\"modality\":\"text\",\"language\":\"en\",\"participantId\":\"1\"}},\"parameters\":{\"projectName\":\"lab09\",\"verbose\":true,\"deploymentName\":\"TimeModelDep\",\"stringIndexType\":\"TextElement_V8\"}}"