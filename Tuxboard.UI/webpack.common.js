const path = require('path');

module.exports = {
    entry: './wwwroot/src/Tuxboard.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/js/'),
        filename: "tuxboard.js"
    }
}
