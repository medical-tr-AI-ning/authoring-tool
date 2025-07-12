#!/usr/bin/env bash

if [[ "$MATTERMOST_NOTIFICATIONS" == "true" ]] && ! [[ -z "$MATTERMOST_URL" ]]; then
    echo "Notifying Mattermost"
    curl -i -X POST -H 'Content-Type: application/json' -d '{"text": "#### :trainwreck: CI-Pipeline - ['$CI_PROJECT_NAME']('$CI_PROJECT_URL')\nA [CI pipeline]('$CI_PIPELINE_URL')  finished successfully on branch ['$CI_COMMIT_REF_NAME']('$CI_PROJECT_URL'/-/tree/'$CI_COMMIT_REF_NAME') (commit ['$CI_COMMIT_SHA']('$CI_PROJECT_URL'/-/commit/'$CI_COMMIT_SHA')).\n\nCheck out the build artifacts [here]('$ARTIFACT_DOWNLOAD_URL').\n@channel"}' $MATTERMOST_URL
else
    echo "Mattermost Notifications disabled"
fi
