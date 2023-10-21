#!/bin/bash

ps -ef | grep XSF_ | grep -v grep | awk '{print $2}' | xargs kill -9