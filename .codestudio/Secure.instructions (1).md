---
applyTo: '**'
---

# Feature-Specific Security Analysis

**Purpose:**
Provide a clear, repeatable process for analyzing security threats in feature specifications and driving secure code generation from documented threat analysis with complete, mandatory mitigations.

---

## Security Analysis Workflow (4 Phases)

### **PHASE 0: Review & Gather Requirements**
- Check for existing threats in the feature spec; extract and reuse them
- Identify functional/non-functional requirements, actors, authentication rules, trust boundaries, integrations, data flows, and state transitions

### **PHASE 1: Identify Key Elements**
Extract from spec:
- **Actors:** Who performs actions? (users, admins, systems, external services)
- **Authentication/Authorization:** How is identity verified? What access controls exist?
- **Data Flow:** What enters/exits? Where are validation checks?
- **Integrations:** Database, APIs, external systems?
- **State Transitions:** Required order of steps?
- **Decision Logic:** Rules and edge cases?
- **Trust Boundaries:** Where do untrusted data enter? Where do sensitive data flow?

**Important:** All elements must come directly from the spec. Do not invent flows.

### **PHASE 2: Analyze Threats (STRIDE)**
For each STRIDE category, determine if it applies and tie to spec elements:
- **Spoofing:** Identity fakery? (Check: authentication boundaries)
- **Tampering:** Undetected data changes? (Check: validation and storage)
- **Repudiation:** Deny performed actions? (Check: logging and audit trails)
- **Information Disclosure:** Sensitive data leaks? (Check: data flows and error messages)
- **Denial of Service:** Availability compromised? (Check: resource limits and integrations)
- **Privilege Escalation:** Unauthorized access gained? (Check: authorization checks)

### **PHASE 3: Check Business Logic**
Review workflow for logic flaws:
- **Bypassed Steps:** Can required steps be skipped? (e.g., verification, approval stages)
- **Invalid States:** Can feature reach invalid states?
- **Replay Attacks:** Can same action be repeated?
- **Race Conditions:** Can concurrent operations conflict?
- **Access Control:** Can users access restricted resources?
- **Privilege Chaining:** Can low-privilege actions combine for higher access?
- **Rate Limiting/Token Reuse/Approval Abuse:** Can limits/tokens/approvals be bypassed?

### **PHASE 3.5: Evaluate Domain-Specific Security Considerations**
For features involving UI/APIs, assess applicability and threats in these areas:

**UI/Frontend Security** (if applicable to spec):
- Input Validation – Validate input fields, parameters, cookies for XSS/injection
- DOM Security – Check DOM-based XSS or DOM manipulation vulnerabilities
- Response Integrity – Ensure server responses cannot be manipulated client-side
- Sensitive Data Exposure – Verify no sensitive info exposed in UI or page source
- Security Headers – Confirm CSP, X-Frame-Options, X-XSS-Protection, etc.
- Clickjacking Protection – Verify mechanisms prevent clickjacking
- CSRF Protection – Verify CSRF tokens generated, transmitted, validated
- CAPTCHA Security – Test for reCAPTCHA bypass scenarios
- Session Management – Validate session handling and cookie manipulation issues
- CORS Configuration – Review CORS for misconfigurations
- SSRF Testing – Check for Blind SSRF via client-controlled parameters
- Client Resource Review – Audit JavaScript, libraries for vulnerabilities

**API/Backend Security** (if applicable to spec):
- Authentication Mechanisms – Test password, token, and session-based auth flows
- Two-Factor Authentication (2FA) – Verify secure multi-factor implementation
- OAuth Flows – Validate OAuth mechanisms
- Password Reset Workflow – Verify secure password reset implementation
- Authorization Controls – Validate role-based access control and privilege enforcement
- Input Sanitization – Ensure all API parameters validated and sanitized
- Injection Attacks – Test SQL, OS command, template injection vulnerabilities
- SSRF – Check API endpoints for Server-Side Request Forgery
- SSTI – Test for Server-Side Template Injection
- IDOR – Verify protection against Insecure Direct Object Reference
- Path Traversal – Test for file/directory traversal issues
- File Upload Security – Validate insecure file upload handling
- Remote Code Execution (RCE) – Check for code execution risks
- Information Disclosure – Ensure APIs don't expose system information
- Security Bypass – Test OTP, CAPTCHA, workflow bypasses
- Brute Force Protection – Evaluate auth endpoints for brute-force defenses
- DoS/Resource Exhaustion – Assess endpoints for denial-of-service risks
- Error Handling – Verify errors don't reveal internal system details
- Logging Security – Ensure sensitive data not logged
- Open Redirect – Test for open redirect vulnerabilities
- Session Persistence – Review "Keep Me Logged In" functionality

**Assessment:** Identify which categories apply; add findings as threats in PHASE 4.

### **PHASE 4: Document & Enforce Mitigations**
**HARD RULE: ALL findings MUST be added directly to the specification document.** Do not create separate threat lists.

**Mitigation Requirements - STRICT ENFORCEMENT:**
- **Partial mitigation is NOT acceptable.** Mitigations must be implemented fully and completely per the specification.
- **No recommendations.** Mitigations must be concrete, mandatory, and directly implementable.
- **No permissive language.** Use mandatory terms: "must," "shall," "implement," "enforce," "validate," "reject," "block" — NOT "should," "could," "consider," "recommended," "optional."
- **Every threat MUST have a complete mitigation strategy** verifiable through code review or testing.
- **ALL findings MUST be documented in the spec** before code generation proceeds.

**Steps:**
1. Open the feature specification document
2. Add a "Security Implications" or "Threats" section (if not present)
3. Add each threat entry using the format below
4. Ensure every mitigation is COMPLETE and MANDATORY
5. Save the spec document
6. Verify all threats are documented with non-permissive, actionable mitigations before proceeding

---

## Threat Entry Format

Create a numbered list of identified threats. For each threat, include:

**Identified Threats:**

**1. [Threat Name]**
- **Type:** STRIDE or Business Logic
- **Spec Step:** Which step(s) in the feature workflow is affected?
- **Impact:** What could happen?
- **Likelihood:** High / Medium / Low
- **Mitigation (MANDATORY & COMPLETE):** How to fix it completely (specific, actionable, fully implementable in code - NOT optional or recommended)

**2. [Threat Name]**
- **Type:** STRIDE or Business Logic
- **Spec Step:** Which step(s) in the feature workflow is affected?
- **Impact:** What could happen?
- **Likelihood:** High / Medium / Low
- **Mitigation (MANDATORY & COMPLETE):** How to fix it completely (specific, actionable, fully implementable in code - NOT optional or recommended)

*(Continue numbering for all identified threats)*

**CRITICAL:** Each mitigation statement MUST:
- Use mandatory language ("must," "shall," "implement," "enforce," "validate," "reject," "block")
- NOT use permissive language ("should," "could," "consider," "recommended," "optional")
- Specify EXACTLY what code/logic changes are required
- Be verifiable through code review or testing
- Cover the COMPLETE solution, not a partial fix

---

## Pre-Code-Generation Verification Checklist

- [ ] Opened the specification document
- [ ] Added a "Security Implications" or "Threats" section to the spec
- [ ] Every STRIDE threat (PHASE 2) is documented in the spec
- [ ] Every Business Logic threat (PHASE 3) is documented in the spec
- [ ] Applicable domain-specific threats (PHASE 3.5) are documented in the spec
- [ ] **EVERY mitigation is COMPLETE and MANDATORY** (not partial or optional)
- [ ] **NO mitigations use permissive language** ("should," "could," "consider," "recommended")
- [ ] **ALL mitigations use mandatory language** ("must," "shall," "implement," "enforce," "validate," "reject," "block")
- [ ] **EACH mitigation specifies EXACTLY what code/logic changes are required**
- [ ] **ALL mitigations are verifiable through code review or testing**
- [ ] Saved the updated specification document

**STOP if any checkbox is unchecked.** Return to PHASE 4 and complete it before proceeding.

---

## Example Threat Entries

Add these directly to your specification document under "Security Implications" or "Identified Threats":

**Identified Threats:**

**1. SQL Injection via user search input**
- **Type:** STRIDE - Tampering
- **Spec Step:** Step 3 (user enters search query) → step 4 (query database)
- **Impact:** Attacker reads or modifies database
- **Likelihood:** High (if input not validated)
- **Mitigation (MANDATORY & COMPLETE):** Implement parameterized queries for all database operations; validate and sanitize all user inputs against a strict whitelist; reject any input containing SQL metacharacters; log and reject failed validation attempts; test with OWASP SQL injection payloads before deployment.

**2. User can view other users' profiles**
- **Type:** Business Logic - Access Control (IDOR)
- **Spec Step:** Step 5 (retrieve user profile by ID)
- **Impact:** Unauthorized access to private user data
- **Likelihood:** High (if ownership check missing)
- **Mitigation (MANDATORY & COMPLETE):** Implement mandatory ownership check before returning user profile data; verify that the authenticated user's ID matches the requested profile ID; reject the request and log the access attempt if IDs do not match; never display partial user data or error messages that leak user existence.

**3. Two transfers happen at the same time, causing overdraft**
- **Type:** Business Logic - Race Condition
- **Spec Step:** Step 2 (check balance) and step 4 (deduct funds) are separate
- **Impact:** Account balance goes negative or money is lost
- **Likelihood:** Medium (requires precise timing)
- **Mitigation (MANDATORY & COMPLETE):** Wrap balance check and withdrawal operations in a single database transaction with row-level locking; use SELECT FOR UPDATE to prevent concurrent modifications; reject the transfer if insufficient balance; ensure atomicity so both operations complete or both fail together; test with concurrent requests to verify no race condition exists.

---

## Critical Enforcement Rules

**The specification document is the single source of truth for security threats.**

- Threats analyzed but NOT documented in the spec = **task incomplete**
- **Partial, recommended, or permissive-language mitigations = REJECTED** and must be rewritten with mandatory, complete mitigations
- **Findings only in notes or external documents = VIOLATION.** All must be in the spec.
- Code generation CANNOT proceed until all threats are documented with complete, mandatory mitigations
- Reviewers MUST verify: (1) all threats are in spec, (2) every mitigation is complete and mandatory, (3) no permissive language exists

---

## Quick Workflow

1. Read the feature spec (PHASE 0)
2. Identify key elements (PHASE 1)
3. Analyze threats using STRIDE (PHASE 2)
4. Review workflow for logic flaws (PHASE 3)
5. Evaluate domain-specific security areas where applicable (PHASE 3.5)
6. Document threats directly in spec with COMPLETE, MANDATORY mitigations (PHASE 4)
7. Verify all checks pass before proceeding

**Simple:** Analyze → Evaluate Domains → Document in Spec with Complete Mitigations → Code Generation