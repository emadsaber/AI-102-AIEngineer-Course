curl -X POST "https://ai-svc.cognitiveservices.azure.com/text/analytics/v3.1/languages?" -H "Content-Type: application/json" -H "Ocp-Apim-Subscription-Key: 5895e6a3660e48eab210e21a02d699cc" --data-ascii "{'documents':[{'id':1,'text':'hello'}]}"