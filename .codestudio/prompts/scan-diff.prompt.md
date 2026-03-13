---
name: scan-diff
description: You are a security workflow orchestrator. Your goal is to run a security scan on all changed files between the current state and the `{{base_branch | default: 'main'}}` branch.
---
**Execution Plan:**
1.  Use the `execute` tool to run `git diff --name-only {{base_branch}}` to get a list of changed files.
2.  For each file in the list, execute the `/scan-file` command, passing the file path to it.
3.  Store the findings from each run.
4.  After scanning all files, execute the `/aggregate-findings` command to consolidate the results into a final report.
5.  Output a summary message indicating the process is complete and where to find the reports.

Begin the process now.