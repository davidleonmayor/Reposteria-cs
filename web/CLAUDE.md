# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

@AGENTS.md

> ⚠️ The note above is load-bearing. **Next.js 16.2.4 + React 19.2** are installed — APIs, file conventions, and metadata shapes may have changed since your training data. Before writing routing, server-action, image, font, or metadata code, read the relevant page in `node_modules/next/dist/docs/` rather than relying on memory.

## Commands

Package manager is **pnpm** (see `pnpm-workspace.yaml`; `sharp` and `unrs-resolver` are pinned as `ignoredBuiltDependencies`).

```bash
pnpm dev      # next dev — http://localhost:3000
pnpm build    # next build
pnpm start    # next start (production)
pnpm lint     # eslint (flat config in eslint.config.mjs)
```

There is **no test runner configured** — don't invent `pnpm test`. Type-check by running `pnpm build` or `pnpm tsc --noEmit`.

## Architecture

A Spanish-language bakery POS UI ("Gestor De Ventas Reposteria"). Currently UI-only — no backend, no DB, no auth provider wired up. All data is mock data in `moks/constants.tsx` (note the typo: `moks`, not `mocks` — used throughout imports as `@/moks/constants`).

### Routing (App Router, route groups)

- `app/(landing)/` — public landing page with its own header/footer.
- `app/(auth)/login`, `app/(auth)/sign-up` — auth pages with a minimal centered layout. `login/services/login.ts` and `config/fetch.ts` exist but are **empty stubs** — the real auth/HTTP layer hasn't been written yet. Signup currently fakes the request with `setTimeout` (see `app/(auth)/sign-up/signup-form.tsx`).
- `app/dashboard/` — authed area with sidebar + header chrome (`app/dashboard/layout.tsx`). Children: `home`, `products`, `products/[id]` (edit form), `sells` (recharts dashboards).
- `app/layout.tsx` mounts the **global `<Toaster />`** (sonner) and **global `<ExitModal />`** — don't re-mount these per page.

### State (Zustand)

Two global stores in `store/`:
- `use-cart.tsx` — `CartItem[]` with `addItem` / `incrementItem` / `decrementItem` / `removeItem` / `clearCart`. Cart UI lives in `components/cart/cart-sheet.tsx` and is also embedded inside `components/dashboard/header.tsx` (duplicated cart sheet — change both, or consolidate, when editing cart UX).
- `use-exit-modal.tsx` — controls the global `ExitModal`.

No persistence middleware — refresh wipes state.

### UI system

- **shadcn/ui** with style `radix-nova` (see `components.json`), Tailwind v4, base color `neutral`, icons via `lucide-react`. CSS variables and `@import "shadcn/tailwind.css"` live in `app/globals.css`.
- **`components/ui/button.tsx` is heavily customized**, not stock shadcn. It uses a Duolingo-style 3D look (`border-b-4 active:border-b-0`) and exposes non-standard variants: `primary`, `primaryOutline`, `secondary`, `secondaryOutline`, `danger`, `dangerOutline`, `super`, `superOutline`, `ghost`, `sidebar`, `sidebarOutline`, `locked`, plus a `rounded` size. When adding new buttons, prefer these existing variants over passing one-off `className` colors.
- Path alias: `@/*` → repo root. Component aliases (per `components.json`): `@/components`, `@/components/ui`, `@/lib`, `@/lib/utils`, `@/hooks`.
- Forms use **react-hook-form + zod** via `@/components/ui/form` (`Form`, `FormField`, `FormItem`, `FormControl`, `FormLabel`, `FormMessage`) — see `signup-form.tsx` for the canonical pattern. Don't hand-roll `useState` form state when adding new forms.

### Local skill — `skills/create-ui-component/`

There's a project skill that enforces strict UI conventions: **reuse `@/components/ui/*` primitives, never re-implement them**; raw HTML (`div`, `section`, `span`, headings) is only for layout/structure with Tailwind utilities. Read `skills/create-ui-component/SKILL.md` before authoring a new page or large component. (Note: its example references `@clerk/nextjs`, but Clerk is **not** actually installed — treat that as illustrative, not a project dependency.)

### Branding inconsistencies to watch

The sidebar still hardcodes the string "Lingo" and uses `/mascot.svg` (`components/dashboard/sidebar.tsx`), and `ExitModal` says "Wait, don't go!" / "End session" → `/learn` — both are template leftovers from a different (Duolingo-style) project. The product is "Repostería". If you're touching nearby code, prefer fixing the branding rather than copying it forward.
