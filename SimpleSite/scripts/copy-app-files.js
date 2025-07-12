#!/usr/bin/env node
/**
 * Copy application files script
 * Cross-platform Node.js implementation with recursive directory support
 */

const fs = require('fs').promises;
const path = require('path');

async function copyDirectory(src, dest) {
  try {
    await fs.mkdir(dest, { recursive: true });
    const entries = await fs.readdir(src, { withFileTypes: true });
    
    for (const entry of entries) {
      const srcPath = path.join(src, entry.name);
      const destPath = path.join(dest, entry.name);
      
      if (entry.isDirectory()) {
        await copyDirectory(srcPath, destPath);
      } else {
        await fs.copyFile(srcPath, destPath);
        console.log(`   ✓ Copied ${path.relative(process.cwd(), srcPath)}`);
      }
    }
  } catch (error) {
    if (error.code === 'ENOENT') {
      console.log(`   ⚠️  Source directory not found: ${src}`);
    } else {
      throw error;
    }
  }
}

async function copyAppFiles() {
  try {
    console.log('📄 Copying application files...');
      // Define directory structure
    const copyOperations = [
      {
        src: path.join('src', 'index.html'),
        dest: path.join('dist', 'index.html'),
        type: 'file'
      },
      {
        src: path.join('src', 'js'),
        dest: path.join('dist', 'js'),
        type: 'directory'
      },
      {
        src: path.join('src', 'css'),
        dest: path.join('dist', 'css'),
        type: 'directory'
      },
      {
        src: path.join('src', 'assets'),
        dest: path.join('dist', 'assets'),
        type: 'directory'
      },      {
        src: path.join('src', 'data'),
        dest: path.join('dist', 'data'),
        type: 'directory'
      },
      {
        src: path.join('src', 'assets', 'favicon.ico'),
        dest: path.join('dist', 'favicon.ico'),
        type: 'file'
      }
    ];
    
    // Process each operation
    for (const { src, dest, type } of copyOperations) {
      try {
        if (type === 'file') {
          // Copy single file
          await fs.mkdir(path.dirname(dest), { recursive: true });
          await fs.copyFile(src, dest);
          console.log(`   ✓ Copied ${path.relative(process.cwd(), src)}`);
        } else if (type === 'directory') {
          // Copy entire directory
          await copyDirectory(src, dest);
        }
      } catch (error) {
        if (error.code === 'ENOENT') {
          console.log(`   ⚠️  Source not found: ${src}`);
        } else {
          console.error(`   ❌ Failed to copy ${src}:`, error.message);
          throw error;
        }
      }
    }
    
    console.log('✅ Application files copied successfully');
    
  } catch (error) {
    console.error('❌ Error copying application files:', error.message);
    process.exit(1);
  }
}

// Export for use in other scripts
module.exports = { copyAppFiles, copyDirectory };

// Run if called directly
if (require.main === module) {
  copyAppFiles();
}
