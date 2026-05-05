# Project Overview

This is a Next.js project bootstrapped with `create-next-app`, leveraging React 19 and TypeScript. It uses the App Router for routing and Tailwind CSS 4 for styling, along with Shadcn UI components for a modern look and feel. The project appears to be in its early stages, with placeholder authentication pages.

**Key Technologies:**
*   **Framework:** Next.js (App Router)
*   **Language:** TypeScript
*   **UI Library:** React 19
*   **Styling:** Tailwind CSS 4, `tw-animate-css`, `shadcn/tailwind.css`
*   **UI Components:** Shadcn UI (using Radix UI Primitives)
*   **State Management:** Zustand
*   **Utilities:** `clsx`, `tailwind-merge`

# Building and Running

## Development Server

To run the development server:

```bash
pnpm dev
```

The application will be accessible at `http://localhost:3000`.

## Build

To build the project for production:

```bash
pnpm build
```

## Start

To start the production server:

```bash
pnpm start
```

## Lint

To run ESLint for code quality checks:

```bash
pnpm lint
```

# Development Conventions

## Project Structure

The project follows the Next.js App Router convention:
*   `app/`: Contains the main application routes and global layout.
    *   `app/globals.css`: Global CSS imports and Tailwind CSS directives.
    *   `app/layout.tsx`: Root layout for the application, including font configuration (`next/font/google`) and a `Toaster` component from `sonner`.
    *   `app/(auth)/`: A route group for authentication-related pages (e.g., `login`, `sing-up`). These currently contain placeholder content.
    *   `app/(landing)/`: A route group for the landing page.
*   `components/`: Contains reusable React components.
    *   `components/ui/`: Specifically for Shadcn UI components (e.g., `button.tsx`, `sonner.tsx`).
*   `lib/`: Contains utility functions.
    *   `lib/utils.ts`: Utility for combining Tailwind CSS classes using `clsx` and `tailwind-merge`.

## Styling

*   **Framework:** Tailwind CSS 4 is used for utility-first styling.
*   **Component Library:** Shadcn UI components are integrated, providing a consistent design system.
*   **Theming:** Dark mode is supported, with CSS variables defined in `app/globals.css` for both light and dark themes.

## Linting

*   **Tool:** ESLint is configured using `eslint-config-next/core-web-vitals` and `eslint-config-next/typescript` for Next.js best practices and TypeScript specific rules.

## TypeScript Configuration

*   The project uses TypeScript with strict checking enabled.
*   Path aliases are configured in `tsconfig.json` for easier imports, notably `@/*` mapping to the project root.
