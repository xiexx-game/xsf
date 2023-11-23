#!/bin/bash

ps -ef | grep xsf-Center | grep -v grep | awk '{print $2}' | xargs kill