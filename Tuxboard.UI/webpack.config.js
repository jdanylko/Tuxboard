const path = require('path');

module.exports = {
    mode: "development",
    entry: './wwwroot/src/Tuxboard.ts',
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                // use: 'awesome-typescript-loader',
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
        filename: "[name]-bundle.js"
    }
}
