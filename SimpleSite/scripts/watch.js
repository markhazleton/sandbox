#!/usr/bin/env node
/**
 * Watch files script
 * Cross-platform Node.js implementation with intelligent file watching
 */

const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');

class FileWatcher {
  constructor(options = {}) {
    this.options = {
      watchDir: options.watchDir || 'src',
      extensions: options.extensions || ['.html', '.js', '.json', '.css'],
      ignorePattern: options.ignorePattern || /node_modules|\.git|dist/,
      debounceMs: options.debounceMs || 100,
      buildCommand: options.buildCommand || ['node', 'scripts/build.js'],
      verbose: options.verbose || false,
      ...options
    };
    
    this.watchers = new Map();
    this.debounceTimeout = null;
    this.building = false;
  }

  log(message, type = 'info') {
    const timestamp = new Date().toISOString().slice(11, 19);
    const emoji = { info: '👀', build: '🔨', success: '✅', error: '❌', change: '📝' }[type];
    console.log(`${emoji} [${timestamp}] ${message}`);
  }

  shouldWatch(filePath) {
    // Check if file extension should be watched
    const ext = path.extname(filePath);
    if (!this.options.extensions.includes(ext)) {
      return false;
    }
    
    // Check if path should be ignored
    if (this.options.ignorePattern.test(filePath)) {
      return false;
    }
    
    return true;
  }

  async runBuild() {
    if (this.building) {
      if (this.options.verbose) {
        this.log('Build already in progress, skipping...', 'info');
      }
      return;
    }

    this.building = true;
    this.log('File changes detected, rebuilding...', 'build');
    
    try {
      const buildProcess = spawn(this.options.buildCommand[0], this.options.buildCommand.slice(1), {
        stdio: this.options.verbose ? 'inherit' : 'pipe',
        shell: true,
        cwd: process.cwd()
      });

      let output = '';
      if (!this.options.verbose) {
        buildProcess.stdout?.on('data', (data) => {
          output += data.toString();
        });
        buildProcess.stderr?.on('data', (data) => {
          output += data.toString();
        });
      }

      buildProcess.on('exit', (code) => {
        this.building = false;
        if (code === 0) {
          this.log('Build completed successfully', 'success');
        } else {
          this.log(`Build failed with exit code ${code}`, 'error');
          if (!this.options.verbose && output) {
            console.log(output);
          }
        }
      });

    } catch (error) {
      this.building = false;
      this.log(`Build error: ${error.message}`, 'error');
    }
  }

  debouncedBuild() {
    if (this.debounceTimeout) {
      clearTimeout(this.debounceTimeout);
    }
    
    this.debounceTimeout = setTimeout(() => {
      this.runBuild();
    }, this.options.debounceMs);
  }

  watchDirectory(dirPath) {
    try {
      const watcher = fs.watch(dirPath, { recursive: true }, (eventType, filename) => {
        if (!filename) return;
        
        const fullPath = path.join(dirPath, filename);
        
        if (this.shouldWatch(fullPath)) {
          if (this.options.verbose) {
            this.log(`${eventType}: ${filename}`, 'change');
          }
          this.debouncedBuild();
        }
      });

      this.watchers.set(dirPath, watcher);
      this.log(`Watching: ${dirPath}`, 'info');
      
    } catch (error) {
      this.log(`Failed to watch ${dirPath}: ${error.message}`, 'error');
    }
  }

  async start() {
    try {
      this.log('Starting file watcher...', 'info');
      this.log(`Extensions: ${this.options.extensions.join(', ')}`, 'info');
      
      // Watch the main directory
      this.watchDirectory(this.options.watchDir);
      
      // Initial build
      await this.runBuild();
      
      this.log('File watcher ready - monitoring for changes...', 'success');
      
      // Keep process alive
      process.stdin.resume();
      
    } catch (error) {
      this.log(`Failed to start watcher: ${error.message}`, 'error');
      process.exit(1);
    }
  }

  stop() {
    this.log('Stopping file watcher...', 'info');
    
    for (const [path, watcher] of this.watchers) {
      watcher.close();
      this.log(`Stopped watching: ${path}`, 'info');
    }
    
    this.watchers.clear();
    
    if (this.debounceTimeout) {
      clearTimeout(this.debounceTimeout);
    }
  }
}

// Export for use in other scripts
module.exports = { FileWatcher };

// Run if called directly
if (require.main === module) {
  const args = process.argv.slice(2);
  const options = {};
  
  // Parse command line arguments
  for (let i = 0; i < args.length; i++) {
    switch (args[i]) {
      case '--watch-dir':
      case '-w':
        options.watchDir = args[++i];
        break;
      case '--extensions':
      case '-e':
        options.extensions = args[++i].split(',').map(ext => ext.trim());
        break;
      case '--verbose':
      case '-v':
        options.verbose = true;
        break;
      case '--debounce':
      case '-d':
        options.debounceMs = parseInt(args[++i]) || 100;
        break;
    }
  }
  
  const watcher = new FileWatcher(options);
  
  // Handle graceful shutdown
  process.on('SIGINT', () => {
    console.log('\n🛑 Shutting down file watcher...');
    watcher.stop();
    process.exit(0);
  });
  
  process.on('SIGTERM', () => {
    console.log('\n🛑 Shutting down file watcher...');
    watcher.stop();
    process.exit(0);
  });
  
  watcher.start();
}
