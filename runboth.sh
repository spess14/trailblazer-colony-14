#!/usr/bin/env bash

tmux new-session "dotnet run --project Content.Client"\; \
  splitw "dotnet run --project Content.Server"
