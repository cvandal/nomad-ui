module.exports = {
    entry: './Views/index.js',
    output: {
        filename: './wwwroot/js/site.js'
    },
    module: {
        loaders: [
            { test: /\.js$/, loader: 'babel-loader', exclude: /node_modules/ },
            { test: /\.jsx$/, loader: 'babel-loader', exclude: /node_modules/ }
        ]
    }
}
