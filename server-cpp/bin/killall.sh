
#!/bin/bash

ps -ef | grep xsf- | grep -v grep | awk '{print $2}' | xargs kill -9

exit 0