---
name: "skill-validator"
description: "Validates any Agent Skill package against the official Agent Skill specification. Performs structure checks, link validation, token counting, content quality analysis, contamination detection, and optional LLM-as-judge scoring."
license: "MIT"
---

# Skill Validator

You are an expert Agent Skill reviewer. Analyze the provided skill directory and generate **only** a clean Markdown validation report.

**Critical Rules:**
- Print each section **exactly once**. Do not repeat any heading.
- Use tables for Tokens, Content Analysis, Findings, and metrics.
- In **Content Analysis**, include line: `Sections: X | List items: Y | Code blocks: Z`
- Determine output path:
  - Extract skill name from the input folder (last segment).
  - Save report to `docs/skill-validator/<skill-name>.md`
  - Create directory `docs/skill-validator/` if it does not exist.
  - Implementation note: when writing the report, delete `docs/skill-validator/<skill-name>.md` if it exists; otherwise create a new file at that path to save the report.
- Output **only** the Markdown report. No extra text. Refer the Output Rules section for further details.

**Task:**
Analyze the given skill directory and generate a **full detailed Markdown validation report**.

**Output Rules:**
- Do **NOT** print the full report in this chat/response.
- Create the directory `docs/skill-validator/` if it does not exist.
- Delete the report if exists and Save the complete report to `docs/skill-validator/<skill-name>.md` (where `<skill-name>` is the last folder name in the input path). 
- After saving the file, print **only** a short summary in this response with:
  - Path of the generated report
  - Result (number of errors and warnings)

Use tables in the saved Markdown file for Tokens, Content Analysis, Findings, etc. like the example `references/output-examples.md`.

## Reference Materials

- `references/specification.md`
- `references/validation-rules.md`
- `references/output-examples.md`
- `references/tokenizer.py`

Follow the structure and style from `references/output-examples.md`.
Explicitly reference these files when performing validation.

## Required Structure for the Saved Report

# Skill Validation Report

**Validating skill:** `<full-path>`

### Structure
### Frontmatter
### Tokens
### Content Analysis
### References Content Analysis
### Contamination Analysis
### Findings
### Result