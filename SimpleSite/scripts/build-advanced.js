#!/usr/bin/env node
// Simplified advanced build script using only Node.js built-ins
const fs = require('fs').promises;
const path = require('path');
const crypto = require('crypto');

class SimpleBuildSystem {
  constructor() {
    this.config = {
      srcDir: 'src',
      distDir: 'dist',
      verbose: process.argv.includes('--verbose')
    };
  }

  log(message, type = 'info') {
    const timestamp = new Date().toISOString().slice(11, 19);
    const emoji = { info: '📝', success: '✅', error: '❌', warn: '⚠️' }[type];
    console.log(`${emoji} [${timestamp}] ${message}`);
  }

  // Calculate file hash for change detection
  async getFileHash(filePath) {
    try {
      const content = await fs.readFile(filePath);
      return crypto.createHash('md5').update(content).digest('hex');
    } catch {
      return null;
    }
  }

  // Smart copy - only copy if file changed
  async smartCopy(src, dest) {
    try {
      const srcHash = await this.getFileHash(src);
      const destHash = await this.getFileHash(dest);
      
      if (srcHash && srcHash === destHash) {
        if (this.config.verbose) {
          this.log(`Skipping unchanged file: ${src}`, 'info');
        }
        return false;
      }

      await fs.mkdir(path.dirname(dest), { recursive: true });
      await fs.copyFile(src, dest);
      this.log(`Copied: ${src} → ${dest}`, 'success');
      return true;
    } catch (error) {
      this.log(`Copy failed: ${src} → ${dest} (${error.message})`, 'error');
      return false;
    }
  }

  // Recursively copy directory contents
  async copyDirectory(srcDir, destDir) {
    try {
      const entries = await fs.readdir(srcDir, { withFileTypes: true });
      let copiedCount = 0;

      for (const entry of entries) {
        const srcPath = path.join(srcDir, entry.name);
        const destPath = path.join(destDir, entry.name);

        if (entry.isDirectory()) {
          await fs.mkdir(destPath, { recursive: true });
          copiedCount += await this.copyDirectory(srcPath, destPath);
        } else {
          if (await this.smartCopy(srcPath, destPath)) {
            copiedCount++;
          }
        }
      }

      return copiedCount;
    } catch (error) {
      this.log(`Directory copy failed: ${srcDir} (${error.message})`, 'error');
      return 0;
    }
  }

  // Generate build manifest
  async generateManifest() {
    try {
      const manifest = {
        buildTime: new Date().toISOString(),
        version: '1.0.0',
        files: [],
        nodeVersion: process.version,
        platform: process.platform
      };

      // Walk through dist directory
      async function addFiles(dir, baseDir) {
        const entries = await fs.readdir(dir, { withFileTypes: true });
        
        for (const entry of entries) {
          const fullPath = path.join(dir, entry.name);
          const relativePath = path.relative(baseDir, fullPath);
          
          if (entry.isDirectory()) {
            await addFiles(fullPath, baseDir);
          } else {
            const stats = await fs.stat(fullPath);
            const content = await fs.readFile(fullPath);
            const hash = crypto.createHash('md5').update(content).digest('hex');
            
            manifest.files.push({
              path: relativePath.replace(/\\/g, '/'), // Normalize path separators
              size: stats.size,
              hash: hash,
              modified: stats.mtime.toISOString()
            });
          }
        }
      }

      await addFiles('dist', 'dist');
      await fs.writeFile('dist/build-manifest.json', JSON.stringify(manifest, null, 2));
      this.log(`Generated manifest with ${manifest.files.length} files`, 'success');
    } catch (error) {
      this.log(`Manifest generation failed: ${error.message}`, 'warn');
    }
  }

  async build() {
    const startTime = Date.now();
    this.log('Starting advanced Node.js build...', 'info');

    try {      // Clean and setup
      await fs.rm('dist', { recursive: true, force: true });
      await fs.mkdir('dist/assets', { recursive: true });
      await fs.mkdir('dist/js', { recursive: true });
      await fs.mkdir('dist/css', { recursive: true });
      await fs.mkdir('dist/data', { recursive: true });

      // Copy main HTML file
      await this.smartCopy('src/index.html', 'dist/index.html');      // Copy Bootstrap assets
      await this.smartCopy('node_modules/bootstrap/dist/css/bootstrap.min.css', 'dist/assets/bootstrap.min.css');
      await this.smartCopy('node_modules/bootstrap/dist/css/bootstrap.min.css.map', 'dist/assets/bootstrap.min.css.map');
      await this.smartCopy('node_modules/bootstrap/dist/js/bootstrap.bundle.min.js', 'dist/assets/bootstrap.bundle.min.js');
      await this.smartCopy('node_modules/bootstrap/dist/js/bootstrap.bundle.min.js.map', 'dist/assets/bootstrap.bundle.min.js.map');

      // Copy application files
      const jsCopied = await this.copyDirectory('src/js', 'dist/js');
      const cssCopied = await this.copyDirectory('src/css', 'dist/css');
      const assetsCopied = await this.copyDirectory('src/assets', 'dist/assets');
      const dataCopied = await this.copyDirectory('src/data', 'dist/data');

      this.log(`Copied ${jsCopied} JS files, ${cssCopied} CSS files, ${assetsCopied} assets, ${dataCopied} data files`, 'info');

      // Generate build manifest
      await this.generateManifest();

      const duration = Date.now() - startTime;
      this.log(`Advanced build completed in ${duration}ms`, 'success');
      
    } catch (error) {
      this.log(`Build failed: ${error.message}`, 'error');
      process.exit(1);
    }
  }
}

// Export for testing
module.exports = SimpleBuildSystem;

// Run if called directly
if (require.main === module) {
  new SimpleBuildSystem().build();
}
