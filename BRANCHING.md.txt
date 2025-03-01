# Branching Strategy

This project follows a Git Flow-inspired branching strategy with some simplifications.

## Main Branches

- `main` - Production-ready code. Only merged into after thorough testing.
- `develop` - Integration branch for features. Always in a working state.

## Supporting Branches

- `feature/` - Feature branches for new development (e.g., `feature/memory-system`)
- `bugfix/` - Bug fix branches (e.g., `bugfix/pathfinding-issue`)
- `release/` - Release preparation (e.g., `release/v0.1.0`)
- `hotfix/` - Urgent fixes for production (e.g., `hotfix/critical-llm-crash`)

## Workflow

1. **Feature Development**
   - Create from: `develop`
   - Merge back to: `develop`
   - Naming: `feature/feature-name`

2. **Bug Fixing**
   - Create from: `develop`
   - Merge back to: `develop`
   - Naming: `bugfix/bug-description`

3. **Release Preparation**
   - Create from: `develop`
   - Merge back to: `develop` and `main`
   - Naming: `release/vX.Y.Z`

4. **Hotfixes**
   - Create from: `main`
   - Merge back to: `develop` and `main`
   - Naming: `hotfix/issue-description`

## Pull Request Process

1. Create PR from your branch to the appropriate target branch
2. Request at least one code review
3. Ensure all tests/checks pass
4. Squash-merge when approved

## Commit Messages

Format: `[Type] Brief description`

Types:
- `[Feature]` - New functionality
- `[Fix]` - Bug fixes
- `[Refactor]` - Code improvements without new features
- `[Style]` - Formatting, comments, no code change
- `[Docs]` - Documentation only
- `[Test]` - Adding or refactoring tests
- `[Perf]` - Performance improvements