#!/bin/bash

# Use a while loop to read each line
while IFS= read -r line || [ -n "$line" ]; do
    # Use awk to split each line into project_name and token
    project_name=$(echo "$line" | awk -F '=' '{print $1}')
    access_token=$(echo "$line" | awk -F '=' '{print $2}')

    # Rewrite file
    sed -i "/$project_name/s#https://#&$project_name:$access_token@#g" "$2"
done < "$1"
