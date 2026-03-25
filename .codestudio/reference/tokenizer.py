"""
Accurate Token Counter for Agent Skills
Uses cl100k_base (same as GPT-4o, o1, etc.) — the tokenizer used by skill-validator.
"""

import sys
import tiktoken
from pathlib import Path

def count_tokens(text: str, model: str = "cl100k_base") -> int:
    enc = tiktoken.get_encoding(model)
    return len(enc.encode(text, disallowed_special=()))

def main():
    skill_dir = Path(sys.argv[1]) if len(sys.argv) > 1 else Path(".")
    
    print("=== Accurate Token Usage (cl100k_base) ===\n")
    
    total = 0
    
    # SKILL.md
    skill_path = skill_dir / "SKILL.md"
    if skill_path.exists():
        text = skill_path.read_text(encoding="utf-8")
        tokens = count_tokens(text)
        print(f"SKILL.md body:          {tokens:5,} tokens")
        total += tokens
    else:
        print("SKILL.md: Not found")
    
    # References
    ref_dir = skill_dir / "references"
    if ref_dir.exists():
        for file in sorted(ref_dir.glob("*.md")):
            text = file.read_text(encoding="utf-8")
            tokens = count_tokens(text)
            print(f"references/{file.name:<25} {tokens:5,} tokens")
            total += tokens
    
    print("-" * 50)
    print(f"Total tokens:           {total:5,} tokens")
    
    if total > 8000:
        print("⚠️  WARNING: Total exceeds recommended limit (~8000 tokens)")
    elif total > 5000:
        print("⚠️  Notice: Approaching recommended limit")

if __name__ == "__main__":
    main()