#!/usr/bin/env node
/**
 * Clean script - Remove dist directory
 * Cross-platform Node.js implementation
 */

const fs = require('fs').promises;
const path = require('path');

async function clean() {
  const distPath = path.join(process.cwd(), 'dist');
  
  try {
    console.log('🧹 Cleaning dist directory...');
    await fs.rm(distPath, { recursive: true, force: true });
    console.log('✅ Dist directory cleaned successfully');
  } catch (error) {
    if (error.code === 'ENOENT') {
      console.log('ℹ️  Dist directory already clean');
    } else {
      console.error('❌ Error cleaning dist directory:', error.message);
      process.exit(1);
    }
  }
}

// Export for use in other scripts
module.exports = { clean };

// Run if called directly
if (require.main === module) {
  clean();
}
