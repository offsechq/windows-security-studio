# Upstream Sync Guide

This repository is a rebranded fork of [HotCakeX/Harden-Windows-Security](https://github.com/HotCakeX/Harden-Windows-Security). We operate independently but selectively port features and fixes from the upstream repository.

## Workflow Overview

A **weekly** automated workflow checks for new upstream commits in the app source directories. When changes are detected, it creates a **draft Pull Request** on a `sync/upstream-YYYY-MM-DD` branch. We do **not** merge upstream directly. We **manually port** relevant logic onto the sync branch, then merge it via the PR.

> **Important:** We only track changes in `AppControl Manager/` and `Harden System Security/` from upstream. Workflow files, wiki, root-level configs, and other upstream directories are ignored by the sync workflow.

## Last Synced SHA

The file `.github/LAST_SYNCED_UPSTREAM_SHA` stores the upstream commit SHA that was last synced. The workflow uses this to determine the diff range, ensuring we never re-process already-reviewed commits.

- When you **merge a sync PR**, the SHA file is updated automatically (the PR includes a commit that bumps it).
- If you need to manually reset the baseline, update the file with the desired upstream commit hash.

## Directory Mappings

| Upstream Directory | Local Directory | Notes |
| :--- | :--- | :--- |
| `AppControl Manager/` | `App Control Studio/` | Core logic, UI, ViewModels. **Heavy Rebranding.** |
| `Harden System Security/` | `System Security Studio/` | Separate component. Rebranded. |
| `AppControl Manager/eXclude/` | `App Control Studio/eXclude/` | Helper tools (PartnerCenter, etc.). |

## Sync Process

### 1. Review the Draft PR
When the workflow creates a sync PR, it contains:
- A summary table with commit and file counts per app
- Commit logs split by app (ACS vs SSS)
- A per-file mapping table showing upstream path → local path
- A link to the full upstream diff
- A review checklist

### 2. Analysis
Review the commits listed in the PR body. Identify **functional changes** (features, bug fixes).
*   **Ignore:** `.github/workflows`, `README.md`, `LICENSE`, repo-specific docs, wiki changes, and root-level files.
*   **Ignore:** ARM64-specific changes — we removed ARM64 support and build x64 only.
*   **Ignore:** Project file renames or solution file changes (unless dependencies changed).
*   **Focus:** `.cs` (Logic), `.xaml` (UI), `.resw` (Strings) inside the app directories.

### 3. Porting Strategy
For each feature/fix:

1.  **Diff the upstream commit:**
    ```bash
    git remote add upstream https://github.com/HotCakeX/Harden-Windows-Security.git
    git fetch upstream
    git show --stat <commit-hash>
    git diff <commit-hash>^..<commit-hash> -- "AppControl Manager/Path/To/File.cs"
    ```
2.  **Apply to the sync branch:**
    *   Check out the sync branch: `git checkout sync/upstream-YYYY-MM-DD`
    *   Open the corresponding file in `App Control Studio/` or `System Security Studio/`.
    *   Apply the logic changes manually.
    *   **Crucial:** Respect local namespaces and class names.
    *   **Crucial:** Do not blindly copy-paste if the file has structural differences.
    *   Commit ported changes to the sync branch.
3.  **Resources:**
    *   If upstream added keys to `Strings/en-US/Resources.resw`, add them to our `Resources.resw`.

### 4. Verification
After porting, push to the sync branch. CI will validate the x64 build automatically.
You can also verify locally:
```bash
dotnet build -r win-x64 "App Control Studio/App Control Studio.csproj"
dotnet build -r win-x64 "System Security Studio/System Security Studio.csproj"
```

### 5. Merge
Once all relevant changes are ported and the build passes:
1. Mark the PR as **Ready for Review** (remove draft status).
2. Complete the checklist items in the PR body.
3. Merge the PR. This updates `LAST_SYNCED_UPSTREAM_SHA` on `main`.

### 6. Cleanup
The sync branch is deleted automatically when the PR is merged. The upstream remote is only used transiently in the workflow; no manual cleanup needed.

## Common Scenarios

*   **Version Updates:** If upstream bumps `version.txt` or `Package.appxmanifest` version, decide if we want to adopt it or stick to our own versioning.
*   **New Files:** If upstream adds a new class file, create it locally in the mapped directory, adjusting the `namespace` to match our project.
*   **Renamed Files:** If upstream renames a file we have, check if we should rename ours or just update the content.
*   **ARM64 Changes:** Skip any upstream changes that are ARM64-specific (publish profiles, conditional ItemGroups, ARM64 build steps). We are x64-only.
*   **No Changes This Week:** If no relevant commits exist, the workflow exits silently — no PR is created.
*   **Existing Open PR:** If a sync PR is already open, the workflow skips creation. Merge or close the existing one first.

## Manual Sync (if needed)
If you need to sync outside the weekly schedule:
1. Go to **Actions** → **Sync Upstream** → **Run workflow** (manual dispatch).
2. The same PR-based flow will execute on demand.
