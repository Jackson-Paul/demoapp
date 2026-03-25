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
2.  Apply the rules defined in below section `# Security Review Instructions`.
3.  Use the prompt in `.codestudio/prompts/scan-file.prompt.md` to analyze the file.
4.  Output any findings as a JSON object adhering to the schema in below section `# Security Review Instructions`. Append the findings to the temporary file `docs/security/raw-findings.json`.

### aggregate-findings
**Description**: (Internal command) Aggregates and filters findings, then generates a final report.
**Instructions**:
1.  Read the raw findings from `docs/security/raw-findings.json`.
2.  Deduplicate the findings. A finding is a duplicate if it has the same `file`, `startLine`, `endLine`, and `class`.
3.  Use the prompt in `.codestudio/prompts/aggregate-findings.prompt.md` to generate a summary.
4.  Write the final, filtered findings to `docs/security/security-review.json` (overwriting the raw data).
5.  Write the human-readable summary to `docs/security/security-review.md`.




# Security Review Instructions

## Core Goal
Your primary goal is to identify potential security vulnerabilities in code with high accuracy. You must follow the rules, schemas, and filtering processes defined below without deviation.

## Rules of Engagement
1.  **Analyze Code Only**: Base your findings solely on the provided source code. Do not speculate about runtime environments or external configurations.
2.  **Adhere to Schema**: Every finding you generate MUST conform to the `Finding JSON Schema` defined below. Findings that do not match the schema will be discarded.
3.  **Apply FP Filtering**: You must apply the Two-Stage False-Positive Filtering process to all findings before finalizing your report.


## Security Categories to Examine
1. **Input Validation & Injection**
   - SQL/ORM injection, NoSQL injection, LDAP injection
   - Command injection (subprocess/`exec`/shell)
   - Path traversal, uncontrolled file write/read, zip slip
   - Template injection (server- and client-side), SSTI
   - XXE, XML bombs (if parsers are used insecurely)

2. **Authentication & Authorization**
   - Auth bypass, weak session handling, CSRF on state-changing routes
   - Privilege escalation (horizontal/vertical), IDOR
   - JWT/OAuth issues (alg=none, no signature check, no audience/issuer checks)

3. **Crypto & Secrets Management**
   - Hardcoded secrets/API keys/tokens
   - Weak algorithms, insecure modes, predictable IV/nonce, poor randomness
   - Certificate pinning/validation bypass, TLS verification disabled

4. **Deserialization & RCE**
   - Insecure deserialization (Pickle, Java/Kryo, YAML unsafe load)
   - Unsafe dynamic execution (`eval`, `Function`, reflection with user input)

5. **Web Security**
   - XSS (reflected/stored/DOM)
   - Open redirect (only if redirect target is user-controlled host/protocol)
   - Clickjacking/CSP issues when clearly exploitable

6. **Data Exposure & Privacy**
   - Overbroad responses, sensitive fields exposure, excessive logging of PII/secrets
   - Insecure temporary storage, debug endpoints leaking internals

7. **Supply Chain & Build**
   - Insecure script execution in build/CI with untrusted inputs
   - Dependency execution via `postinstall`/hooks with untrusted sources

8. **Cloud/IaC & Secrets in Infra**
   - Publicly exposed resources in Terraform/ARM/K8s (if clearly exploitable)
   - Insecure security groups/ingress rules; wildcard principals
   - K8s risky settings: hostPath, privileged, CAP_SYS_ADMIN, runAsRoot

9. **Language/Framework-Specific**
   - Python: unsafe `yaml.load`, `pickle`, `subprocess` with shell=True, Flask `debug=True`
   - Node/TS: `child_process` with untrusted args, `eval`, template engines
   - Java: deserialization gadgets, XML parsers without secure features
   - .NET: insecure `JsonSerializerSettings` allowing type name handling, Process.Start with concatenated input


---

## Finding JSON Schema
All security findings must be returned as a JSON object with the following structure. Fields are mandatory unless specified otherwise.

```json
{
  "title": "A brief, descriptive title of the vulnerability.",
  "file": "The full path to the affected file.",
  "startLine": "The starting line number of the vulnerable code.",
  "endLine": "The ending line number of the vulnerable code.",
  "severity": "High | Medium | Low",
  "class": "The CWE or vulnerability category (e.g., 'CWE-79: Improper Neutralization of Input During Web Page Generation (Cross-site Scripting)', 'Hardcoded Secret').",
  "description": "A detailed explanation of the vulnerability, its potential impact, and why the code is considered insecure.",
  "exploitScenario": "A concise, concrete exploitation path (1–2 sentences) grounded in the code shown.",
  "suggestion": "A concrete recommendation on how to fix the vulnerability, including code examples if possible.",
  "confidence": "A numeric score between 0.0 and 1.0 representing exploitability confidence."
}
```

> **Notes**
> - `severity` normalized to include **Critical** and **Informational** for better bucketing.
> - `category` lets you group findings for dashboards.
> - `exploitScenario` forces concrete, actionable context (improves developer trust).
> - `confidence` enables deterministic gating.

---

## 2) Add **Confidence Gate** + **Hard Exclusions** + **Precedents** (from claude)

> Insert this section under **Security Review Instructions** after “Rules of Engagement”.

```md
### Confidence & Quality Gates

- You MUST assign `confidence` ∈ [0.0, 1.0] to each finding.
- DROP any finding with `confidence` < **0.8** (do not emit in outputs).
- Prefer concrete exploit paths over theoretical risks. If exploitation requires unspecified conditions or external systems not present in code, lower confidence accordingly and DROP if < 0.8.

### Hard Exclusions (DROP unless a specific, concrete exploit path is shown)

- Denial of Service / resource exhaustion / rate limiting issues without a direct, reproducible exploit chain tied to code changes
- Outdated dependency versions/transitive CVEs (handled separately in SCA)
- Memory safety concerns in memory-safe languages (unless unsafe FFI/`unsafe` blocks shown)
- Logging of non-PII/non-secret input
- Client-side authorization or permission checks (server must enforce)
- Generic best-practice nits without security impact (e.g., “deprecated API” without risk)
- GitHub Actions/CI YAML concerns lacking a concrete untrusted input path
- Regex DoS, tabnabbing, XS-Leaks, prototype pollution, open redirects—unless the exploit path is explicit and high-confidence
- Unit tests, mocks, or example/demo code unless shipped in production path

### Precedents (Normalize noise and interpretation)

- **React/Angular XSS**: Only valid with explicit unsafe APIs (e.g., `dangerouslySetInnerHTML`, `bypassSecurityTrustHtml`).
- **SSRF**: Requires control over host/protocol (path-only control is insufficient).
- **UUIDs**: Assume unguessable; do not require additional validation.
- **Environment variables & CLI flags**: Treated as trusted for this review context.
- **GitHub Actions**: Require a specific, untrusted input trigger and insecure use of `run`, `checkout`, `pull_request_target`, or artifact injection to report.



## Two-Stage False-Positive (FP) Filtering Process
You must apply this two-stage process to validate every potential finding.

**Stage A: Hard Rule Filtering (Automated Check)**
Drop any finding that fails these checks:

-Missing Location: Does the finding have a file and a valid startLine number? If not, drop it.
-Missing Classification: Does the finding have a class and a severity? If not, drop it.
-Incomplete Description: Does the finding have a non-empty description and suggestion? If not, drop it.
- Must have: `confidence` ∈ [0.0, 1.0].

**Stage B: Self-Challenge Filtering (Reasoning Check)**
For each finding that passes Stage A, you must actively try to prove it is a false positive. Ask yourself these questions:

-Is this test code? Does the code appear in a test file, mock, or clearly non-production context? If so, it's likely a false positive. Drop it.
-Is the finding unreachable? Is the vulnerable code commented out, part of a dead code block, or otherwise unreachable? If so, drop it.
-Is there mitigating context? Does the surrounding code contain sanitization, validation, or other controls that neutralize the identified vulnerability? For example, is user input being properly escaped before being rendered, even if the finding flags a potential XSS? If a mitigating factor exists, drop the finding.
-Is this a weak or non-actionable finding? Is the finding based on a generic best practice that doesn't pose a direct security risk in this specific context (e.g., "use of a deprecated function" without security impact)? If so, drop it.
Only findings that pass both Stage A and Stage B should be included in the final report.


