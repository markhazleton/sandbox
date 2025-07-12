#!/usr/bin/env node
/**
 * Copy Bootstrap assets script
 * Cross-platform Node.js implementation
 */

const fs = require('fs').promises;
const path = require('path');

async function copyBootstrap() {
  try {
    console.log('🎨 Copying Bootstrap assets...');
    
    // Ensure assets directory exists
    const assetsDir = path.join(process.cwd(), 'dist', 'assets');
    await fs.mkdir(assetsDir, { recursive: true });
      // Define Bootstrap files to copy
    const bootstrapFiles = [
      {
        src: path.join('node_modules', 'bootstrap', 'dist', 'css', 'bootstrap.min.css'),
        dest: path.join('dist', 'assets', 'bootstrap.min.css')
      },
      {
        src: path.join('node_modules', 'bootstrap', 'dist', 'css', 'bootstrap.min.css.map'),
        dest: path.join('dist', 'assets', 'bootstrap.min.css.map')
      },
      {
        src: path.join('node_modules', 'bootstrap', 'dist', 'js', 'bootstrap.bundle.min.js'),
        dest: path.join('dist', 'assets', 'bootstrap.bundle.min.js')
      },
      {
        src: path.join('node_modules', 'bootstrap', 'dist', 'js', 'bootstrap.bundle.min.js.map'),
        dest: path.join('dist', 'assets', 'bootstrap.bundle.min.js.map')
      }
    ];
      // Copy each file
    for (const { src, dest } of bootstrapFiles) {
      try {
        if (await fs.access(src).then(() => true).catch(() => false)) {
          await fs.copyFile(src, dest);
          console.log(`   ✓ Copied ${path.basename(src)}`);
        } else {
          console.log(`   ⚠️  Source file not found: ${src}`);
        }
      } catch (error) {
        console.error(`   ❌ Failed to copy ${src}:`, error.message);
        throw error;
      }
    }
    
    console.log('✅ Bootstrap assets copied successfully');
    
  } catch (error) {
    console.error('❌ Error copying Bootstrap assets:', error.message);
    process.exit(1);
  }
}

// Export for use in other scripts
module.exports = { copyBootstrap };

// Run if called directly
if (require.main === module) {
  copyBootstrap();
}
