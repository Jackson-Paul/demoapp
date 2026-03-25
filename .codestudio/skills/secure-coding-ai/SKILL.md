---
name: secure-coding-ai
description: A skill for generating secure code from a feature specification. It analyzes the spec for security implications, generates code with mitigations, and verifies compliance. Use this skill when you are asked to implement, generate, or modify code based on a spec document.
---
# Secure Coding AI Skill

## Overview

This skill ensures that all code generation is done with a security-first approach. It follows a strict, multi-step process:

1.  **Analyze:** Identify security threats and define mitigations based on a feature spec.
2.  **Generate:** Write secure code that implements the defined mitigations.
3.  **Verify:** Ensure that the generated code covers all identified threats.
4.  **Reconcile:** Re-analyze a feature spec after changes have been made.

## Command Router

Based on the user's prompt, determine which of the following commands to execute.

*   If the user asks to **analyze** a feature, use the `analyze-feature.md` prompt.
*   If the user asks to **generate** or **implement** code, first check if a `security-implications.json` file exists for the feature. If it does, use the `generate-secure-code.md` prompt. If not, inform the user that the feature must be analyzed first.
*   If the user asks to **verify** the code, use the `verify-coverage.md` prompt.
*   If the user asks to **reconcile** changes, use the `reconcile-changes.md` prompt.

## File I/O

This skill requires read and write access to the file system to consume feature specs and produce security artifacts. All generated artifacts should be stored in the `docs/security/<feature-name>/` directory.