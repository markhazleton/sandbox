#!/usr/bin/env node
/**
 * Start development server script
 * Cross-platform Node.js implementation
 */

const { spawn } = require('child_process');
const path = require('path');
const net = require('net');
const fs = require('fs');
const { build } = require('./build.js');

function findAvailablePort(startPort = 3000) {
  return new Promise((resolve) => {
    const server = net.createServer();
    
    server.listen(startPort, () => {
      const port = server.address().port;
      server.close(() => resolve(port));
    });
    
    server.on('error', () => {
      // Port is in use, try the next one
      resolve(findAvailablePort(startPort + 1));
    });
  });
}

async function ensureBuilt() {
  if (!fs.existsSync('dist') || !fs.existsSync('dist/index.html')) {
    console.log('📦 Building project first...');
    await build();
  }
}

async function startServer(options = {}) {
  const {
    port = 3000,
    cacheControl = true,
    open = true,
    directory = 'dist'
  } = options;
  
  try {
    console.log('🚀 Starting development server...');
    console.log(`   📁 Serving: ${directory}`);
    console.log(`   🌐 Port: ${port}`);
    console.log(`   📋 Cache: ${cacheControl ? 'enabled' : 'disabled'}`);
    console.log(`   🔗 Auto-open: ${open ? 'yes' : 'no'}`);
    
    // Build command arguments
    const args = [
      directory,
      '-p', port.toString()
    ];
    
    // Add cache control
    if (!cacheControl) {
      args.push('-c-1');
    }
    
    // Add auto-open
    if (open) {
      args.push('-o');
    }
    
    // Spawn http-server process
    const serverProcess = spawn('npx', ['http-server', ...args], {
      stdio: 'inherit',
      shell: true,
      cwd: process.cwd()
    });
    
    // Handle process events
    serverProcess.on('error', (error) => {
      console.error('❌ Failed to start server:', error.message);
      process.exit(1);
    });
    
    serverProcess.on('exit', (code, signal) => {
      if (code !== 0 && code !== null) {
        console.error(`❌ Server exited with code ${code}`);
        process.exit(code);
      }
      if (signal) {
        console.log(`🛑 Server terminated by signal ${signal}`);
      }
    });
    
    // Handle graceful shutdown
    process.on('SIGINT', () => {
      console.log('\n🛑 Shutting down server...');
      serverProcess.kill('SIGINT');
    });
    
    process.on('SIGTERM', () => {
      console.log('\n🛑 Shutting down server...');
      serverProcess.kill('SIGTERM');
    });
    
    console.log(`✅ Server starting at http://localhost:${port}`);
    
  } catch (error) {
    console.error('❌ Error starting server:', error.message);
    process.exit(1);
  }
}

// Export for use in other scripts
module.exports = { startServer };

// Run if called directly with argument parsing
if (require.main === module) {
  const args = process.argv.slice(2);
  const options = {};
  
  // Parse command line arguments
  for (let i = 0; i < args.length; i++) {
    switch (args[i]) {
      case '--port':
      case '-p':
        options.port = parseInt(args[++i]) || 3000;
        break;
      case '--no-cache':
      case '-c':
        options.cacheControl = false;
        break;
      case '--no-open':
        options.open = false;
        break;
      case '--dir':
      case '-d':
        options.directory = args[++i] || 'dist';
        break;
      default:
        // Handle positional arguments (like port numbers)
        const num = parseInt(args[i]);
        if (!isNaN(num) && num > 0 && num < 65536) {
          options.port = num;
        }
        break;
    }
  }
    // Auto-find available port if the default is in use
  (async () => {
    try {
      // Ensure project is built
      await ensureBuilt();
      
      if (!options.port) {
        options.port = await findAvailablePort(3000);
      } else {
        // Check if the requested port is available
        try {
          const testPort = await findAvailablePort(options.port);
          if (testPort !== options.port) {
            console.log(`⚠️  Port ${options.port} is in use, using port ${testPort} instead`);
            options.port = testPort;
          }
        } catch (error) {
          options.port = await findAvailablePort(3000);
        }
      }
      
      startServer(options);
    } catch (error) {
      console.error('❌ Error starting development server:', error.message);
      process.exit(1);
    }
  })();
}
