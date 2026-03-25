---
title: Clean Report Template
---

# Skill Validation Report

**Validating skill:** .codestudio/skills/security-review

### Structure
- SKILL.md found
- References folder valid
- No orphan files

### Frontmatter
- name: "security-review" — valid
- description: Present and concise
- license: MIT — valid

### Tokens

| File                                | Words | Tokens |
|-------------------------------------|--------|-------|
| SKILL.md                            |  1,612 | 2,145 |
| references/scan-file.prompt.md      |  278   | 369   |
| references/scan-diff.prompt.md      |  208   | 277   |
| references/aggregate-findings.prompt.md | 382 | 508 |
| **Total**                           | **2,480** | **3,999** |

### Content Analysis

| Metric                    | Value | Assessment |
|---------------------------|-------|------------|
| Word count                | 1,612 | Good       |
| Imperative ratio          | 0.41  | Good       |
| Information density       | 0.37  | Good       |
| Instruction specificity   | 0.82  | Strong     |

Sections: 15 | List items: 74 | Code blocks: 0

### References Content Analysis
All reference files are focused, well-structured, and explicitly referenced from SKILL.md.

### Contamination Analysis
- Contamination level: low (score: 0.12)
- Primary language: English
- Scope breadth: 1 (focused on security review)

### Findings

| Type    | Severity | Description                                      | Location                              | Suggested Fix |
|---------|----------|--------------------------------------------------|---------------------------------------|---------------|
| Warning | Medium   | Token count approaching recommended limit        | SKILL.md                              | Consider trimming non-essential content |

### Result
0 errors, 1 warning. Skill is ready for publication with minor improvements.