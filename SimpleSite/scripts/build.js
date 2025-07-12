#!/usr/bin/env node
// Node.js build script example
const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

console.log('🔨 Starting Node.js build process...');

async function cleanDist() {
  console.log('🧹 Cleaning dist directory...');
  try {
    if (fs.existsSync('dist')) {
      fs.rmSync('dist', { recursive: true, force: true });
    }
  } catch (error) {
    console.error('❌ Error cleaning dist:', error.message);
    process.exit(1);
  }
}

async function createDirectories() {
  console.log('📁 Creating directory structure...');
  const dirs = ['dist', 'dist/assets', 'dist/js', 'dist/css', 'dist/data'];
  
  dirs.forEach(dir => {
    if (!fs.existsSync(dir)) {
      fs.mkdirSync(dir, { recursive: true });
    }
  });
}

async function copyBootstrapAssets() {
  console.log('🎨 Copying Bootstrap assets...');
  
  const bootstrapFiles = [
    {
      src: 'node_modules/bootstrap/dist/css/bootstrap.min.css',
      dest: 'dist/assets/bootstrap.min.css'
    },
    {
      src: 'node_modules/bootstrap/dist/css/bootstrap.min.css.map',
      dest: 'dist/assets/bootstrap.min.css.map'
    },
    {
      src: 'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js', 
      dest: 'dist/assets/bootstrap.bundle.min.js'
    },
    {
      src: 'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js.map',
      dest: 'dist/assets/bootstrap.bundle.min.js.map'
    }
  ];
  bootstrapFiles.forEach(({ src, dest }) => {
    try {
      if (fs.existsSync(src)) {
        fs.copyFileSync(src, dest);
        console.log(`   ✓ Copied ${path.basename(src)}`);
      } else {
        console.warn(`   ⚠️  Source file not found: ${src}`);
      }
    } catch (error) {
      console.error(`❌ Error copying ${src}:`, error.message);
      process.exit(1);
    }
  });
}

async function copyAppFiles() {
  console.log('📄 Copying application files...');
    const copyOperations = [
    { src: 'src/index.html', dest: 'dist/index.html' },
    { src: 'src/js/script.js', dest: 'dist/js/script.js' },
    { src: 'src/css/hero-gradient.css', dest: 'dist/css/hero-gradient.css' },
    { src: 'src/data/slides.json', dest: 'dist/data/slides.json' },
    { src: 'src/data/simple-slides.json', dest: 'dist/data/simple-slides.json' }
  ];

  copyOperations.forEach(({ src, dest }) => {
    try {
      if (fs.existsSync(src)) {
        fs.copyFileSync(src, dest);
      } else {
        console.warn(`⚠️  Source file not found: ${src}`);
      }
    } catch (error) {
      console.error(`❌ Error copying ${src}:`, error.message);
      process.exit(1);
    }
  });
}

async function copyAssets() {
  console.log('🖼️ Copying assets...');
  
  const assetsDir = 'src/assets';
  const distAssetsDir = 'dist/assets';
  
  try {
    if (fs.existsSync(assetsDir)) {
      const files = fs.readdirSync(assetsDir);
      files.forEach(file => {
        const srcPath = path.join(assetsDir, file);
        const destPath = path.join(distAssetsDir, file);
        
        if (fs.statSync(srcPath).isFile()) {
          fs.copyFileSync(srcPath, destPath);
          console.log(`   ✅ Copied: ${file}`);
          
          // Also copy favicon.ico to root for browser default requests
          if (file === 'favicon.ico') {
            fs.copyFileSync(srcPath, path.join('dist', 'favicon.ico'));
            console.log(`   ✅ Copied favicon.ico to root`);
          }
        }
      });
    }
  } catch (error) {
    console.error('❌ Error copying assets:', error.message);
    process.exit(1);
  }
}

async function build() {
  try {
    await cleanDist();
    await createDirectories();
    await copyBootstrapAssets();
    await copyAssets();
    await copyAppFiles();
    console.log('✅ Node.js build completed successfully!');
  } catch (error) {
    console.error('❌ Build failed:', error.message);
    process.exit(1);
  }
}

// Run if called directly
if (require.main === module) {
  build();
}

module.exports = { build, cleanDist, copyBootstrapAssets, copyAssets, copyAppFiles };
