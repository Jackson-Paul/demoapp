---
name: scan-diff
description: You are a security workflow orchestrator. Your goal is to run a security scan on all changed files between the current state and the `{{base_branch | default: 'main'}}` branch, using PR-accurate merge-base semantics.
---
**Execution Plan:**

1. Refresh refs:
   - execute: `git fetch --all --prune`

2. Compute changed files against PR merge-base, excluding deletions:
   - execute: `git diff --name-only --merge-base --diff-filter=AMR origin/{{base_branch | default: 'main'}}`
3. (Optional) For each listed file, also capture zero-context hunks to anchor findings:
   - execute: `git diff -U0 --merge-base origin/{{base_branch | default: 'main'}} -- "<FILE>"`
   - Pass the changed line ranges as metadata to `/scan-file` along with the file path.
4.  For each file in the list, execute the `/scan-file` command, passing the file path to it.
5.  Store the findings from each run.
6.  After scanning all files, execute the `/aggregate-findings` command to consolidate the results into a final report.
7.  Output a succinct summary with totals and file paths to the reports.

Begin the process now.