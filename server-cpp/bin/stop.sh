
#!/bin/bash

ps -ef | grep xsf-center | grep -v grep |awk '{print $2}' | xargs kill

exit 0