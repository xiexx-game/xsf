#!/bin/bash

ps -ef | grep XSF_Center | grep -v grep | awk '{print $2}' | xargs kill