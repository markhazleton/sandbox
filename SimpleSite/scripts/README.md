# Build Scripts

This folder contains Node.js build scripts for the SimpleSite project. The scripts handle copying source files from the `src/` folder to the `dist/` folder with the proper structure, providing cross-platform compatibility and TypeScript support.

## Scripts Overview

### Node.js Scripts

- **`build.js`** - Main build script that orchestrates the entire build process
  - Cleans the dist directory
  - Creates the proper folder structure
  - Copies Bootstrap assets
  - Copies application files from src/ to dist/
- **`build-advanced.js`** - Advanced build script with additional features
  - Smart copying with change detection
  - Optional minification and optimization
  - Build manifests and detailed logging
- **`clean.js`** - Cross-platform directory cleaning
- **`copy-bootstrap.js`** - Copies Bootstrap CSS and JS to dist/assets/
- **`copy-app-files.js`** - Copies application files maintaining folder structure
- **`start.js`** - Development server with configurable options
- **`watch.js`** - Intelligent file watcher with debouncing

## Usage

The scripts are called from package.json using npm commands:

```bash
# Main build process
npm run build

# Advanced build with optimization
npm run build:advanced

# Clean only
npm run clean

# Build and start server
npm run start

# Build and start dev server (no caching)
npm run dev

# Watch for changes and rebuild
npm run watch
```

## Script Details

### build.js

The main Node.js build script that:

1. **Cleans** - Removes existing dist folder
2. **Creates Structure** - Makes dist/, dist/assets/, dist/js/, dist/css/
3. **Copies Bootstrap** - Copies bootstrap.min.css and bootstrap.bundle.min.js to dist/assets/
4. **Copies App Files** - Copies all files from src/ subfolders to dist/ maintaining structure
   - src/index.html → dist/index.html
   - src/js/* → dist/js/
   - src/css/* → dist/css/
   - src/assets/* → dist/assets/

### build-advanced.js

Enhanced build script with additional features:

- Smart copying with change detection
- Optional file minification
- Image optimization
- Build manifests with timestamps
- Detailed logging and progress indicators

### watch.js

Intelligent file watcher that:

- Monitors src/ directory for changes
- Debounces file changes to prevent excessive rebuilds
- Automatically triggers builds when files are modified
- Provides real-time feedback on build status

### Benefits of Node.js Build System

1. **Cross-platform** - Works identically on Windows, macOS, and Linux
2. **TypeScript Compatible** - Full support for TypeScript build processes
3. **Modularity** - Each script has a single responsibility
4. **Maintainability** - Easy to modify individual build steps
5. **Performance** - Fast file operations with Node.js fs API
6. **Modern JavaScript** - Uses ES6+ features and async/await
7. **Error Handling** - Comprehensive error handling with colored output
8. **Flexibility** - Command-line arguments for script configuration

## Adding New Scripts

To add a new build step:

1. Create a new .js file in this scripts/ folder
2. Use the existing scripts as templates for consistent error handling
3. Add the script call to build.js or create a new npm script in package.json
4. Test the script independently before integrating

## Dependencies

The build system uses only Node.js built-in modules:

- `fs/promises` - File system operations
- `path` - Cross-platform path handling
- `process` - Command-line arguments and exit codes

No external npm dependencies are required for the build process, ensuring fast installation and fewer security vulnerabilities.
