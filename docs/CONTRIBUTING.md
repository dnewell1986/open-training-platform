# Contributing

Open Training Platform is early in development, so the contribution process should stay simple and predictable. These conventions are meant to make branch history easy to scan and commit messages useful later.

## Branch Naming

Use lowercase branch names with hyphen-separated words.

```text
<type>/<short-description>
```

Recommended branch types:

| Type | Use For | Example |
| --- | --- | --- |
| `feature` | New user-facing or platform capability | `feature/course-upload` |
| `fix` | Bug fixes | `fix/scorm-launch-path` |
| `docs` | Documentation-only changes | `docs/branch-commit-guidelines` |
| `refactor` | Code restructuring without behavior changes | `refactor/course-storage-service` |
| `test` | Test-only changes | `test/manifest-parser-fixtures` |
| `chore` | Tooling, dependency, or maintenance work | `chore/central-package-versions` |

If a branch maps to an issue, include the issue number after the type.

```text
feature/123-course-upload
fix/124-runtime-save
```

## Commit Messages

Use a small conventional commit style:

```text
<type>(<scope>): <summary>
```

The scope is optional but encouraged when it makes the affected area obvious.

Examples:

```text
feature(courses): add SCORM package upload endpoint
fix(player): persist completion status on course exit
docs(architecture): describe clean backend project layout
chore(backend): add central package version management
```

Recommended commit types:

| Type | Use For |
| --- | --- |
| `feature` | New functionality |
| `fix` | Bug fixes |
| `docs` | Documentation changes |
| `refactor` | Internal restructuring without behavior changes |
| `test` | Adding or updating tests |
| `chore` | Tooling, build, dependency, or housekeeping changes |

Recommended scopes:

| Scope | Area |
| --- | --- |
| `api` | ASP.NET Core API |
| `domain` | Domain project |
| `application` | Application project |
| `infrastructure` | Infrastructure project |
| `backend` | Backend-wide changes |
| `ui` | React application |
| `courses` | Course management |
| `player` | Course player and SCORM runtime |
| `docs` | Documentation |
| `architecture` | Architecture decisions and diagrams |

## Commit Message Rules

- Use the imperative mood: `add`, `fix`, `update`, `remove`.
- Keep the first line under 72 characters when practical.
- Do not end the summary with a period.
- Keep each commit focused on one logical change.
- Add a body when the reason for the change is not obvious from the summary.

Example with a body:

```text
feature(player): add initial SCORM runtime adapter

Adds a browser-side adapter that receives SCORM API calls from launched
course content and prepares them for persistence through the backend API.
```

## Pull Request Titles

Pull request titles should follow the same pattern as commit messages:

```text
feature(courses): add SCORM package upload
```

This keeps the project history readable when pull requests are squash-merged.
