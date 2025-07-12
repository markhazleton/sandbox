const fs = require('fs');
const pug = require('pug');

// Path to your .pug file
const pugFilePath = 'src/yourFile.pug';

try {
    // Read .pug file content
    const pugContent = fs.readFileSync(pugFilePath, 'utf8');

    // Compile the Pug template to a function
    const compileFunction = pug.compile(pugContent);

    // Render the Pug template to HTML
    const html = compileFunction();

    // Output the HTML to console or write it to a file
    console.log(html);
    fs.writeFileSync('docs/output.html', html);
} catch (error) {
    console.error('Error processing Pug file:', error);
}
