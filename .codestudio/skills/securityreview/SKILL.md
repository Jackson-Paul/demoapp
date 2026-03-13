---
name: securityreview
description: A skill for performing a comprehensive security review on code changes. It identifies potential vulnerabilities, applies a rigorous false-positive filtering process, and generates a structured report of the findings.

---
## Goal
To act as an automated security analyst that reviews code diffs for security flaws based on a defined set of rules and best practices. The primary goal is to provide high-fidelity, actionable security findings while minimizing noise from false positives.

## Tools
- `read`: To read the content of files identified in the code diff.
- `search`: To find related code patterns or definitions within the workspace.
- `execute`: To run `git diff` to identify the code changes to be reviewed.
- `edit`: To create and modify report files.

## Commands

### /scan-diff
**Description**: Scans the staged git diff for potential security vulnerabilities.
**Usage**: `/scan-diff [base_branch]`
**Example**: `/scan-diff main`
**Instructions**:
1.  Use the `execute` tool to run `git diff --name-only [base_branch]` to get a list of changed file paths.
2.  Initialize an empty temporary file at `docs/security/raw-findings.json`.
3.  For each file path in the list, invoke the `scan-file` command with the file path as an argument.
4.  Once all files are scanned, invoke the `aggregate-findings` command to consolidate the results.

### scan-file
**Description**: (Internal command) Scans a single file for security vulnerabilities.
**Instructions**:
1.  Read the file content using the `read` tool.
2.  Apply the rules defined in `.codestudio/codestudio-instructions.md`.
3.  Use the prompt in `.codestudio/prompts/scan-file.prompt.md` to analyze the file.
4.  Output any findings as a JSON object adhering to the schema in `codestudio-instructions.md`. Append the findings to the temporary file `docs/security/raw-findings.json`.

### aggregate-findings
**Description**: (Internal command) Aggregates and filters findings, then generates a final report.
**Instructions**:
1.  Read the raw findings from `docs/security/raw-findings.json`.
2.  Deduplicate the findings. A finding is a duplicate if it has the same `file`, `startLine`, `endLine`, and `class`.
3.  Use the prompt in `.codestudio/prompts/aggregate-findings.prompt.md` to generate a summary.
4.  Write the final, filtered findings to `docs/security/security-review.json` (overwriting the raw data).
5.  Write the human-readable summary to `docs/security/security-review.md`.