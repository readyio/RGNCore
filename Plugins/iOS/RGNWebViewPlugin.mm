#import <WebKit/WebKit.h>

static NSString *URLScheme = @"urlscheme_set_from_c_sharp";
extern "C" void _RGNWebViewPlugin_SetURLScheme(const char *urlScheme) {
    URLScheme = [NSString stringWithUTF8String:urlScheme];
}

static NSString *BackButtonText = @"Close";
extern "C" void _RGNWebViewPlugin_SetBackButtonText(const char *backButtonText) {
    BackButtonText = [NSString stringWithUTF8String:backButtonText];
}

@interface RGNWebViewPlugin : NSObject <WKNavigationDelegate>
@property (nonatomic, retain) WKWebView *webView;
@property (nonatomic, retain) UIButton *closeButton;
@end

@implementation RGNWebViewPlugin

- (instancetype)init {
    self = [super init];
    if (self) {
        _webView = [[WKWebView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
        _webView.navigationDelegate = self;
        // Add a close button
        _closeButton = [UIButton buttonWithType:UIButtonTypeSystem]; // Use the system button type for a more standard look
        [_closeButton setTitle:BackButtonText forState:UIControlStateNormal]; // "Back" is a more standard label for a back button
        [_closeButton setTitleColor:[UIColor blackColor] forState:UIControlStateNormal];
        _closeButton.backgroundColor = [UIColor clearColor]; // Clear the background color for a more standard look
        [_closeButton addTarget:self action:@selector(closeButtonTapped) forControlEvents:UIControlEventTouchUpInside];
        _closeButton.frame = CGRectMake(20, 20, 80, 40); // Move the button to the top-left of the screen
        [_closeButton setContentHorizontalAlignment:UIControlContentHorizontalAlignmentLeft]; // Align the text to the left of the button
        // Get the Unity's UIWindow
        UIWindow *window = [[UIApplication sharedApplication] keyWindow];
        // Add the webView and the close button to the UIWindow's root view controller's view
        [window.rootViewController.view addSubview:_webView];
        [window.rootViewController.view addSubview:_closeButton];
        // Initially, we hide the webView and the close button until a URL needs to be opened
        _webView.hidden = YES;
        _closeButton.hidden = YES;
    }
    return self;
}

- (void)openURL:(NSString*)url {
    NSURL *nsUrl = [NSURL URLWithString:url];
    NSURLRequest *request = [NSURLRequest requestWithURL:nsUrl];
    [self.webView loadRequest:request];
    // Show the webView when opening a URL
    self.webView.hidden = NO;
    self.closeButton.hidden = NO;
}

- (void)closeButtonTapped {
    // Hide the webView and the close button when the close button is tapped
    self.webView.hidden = YES;
    self.closeButton.hidden = YES;
    NSString *urlToOpenString = [URLScheme stringByAppendingString:@"/cancelled"];
    NSURL *urlToOpen = [NSURL URLWithString:urlToOpenString];
    [[UIApplication sharedApplication] openURL:urlToOpen options:@{} completionHandler:nil];
}

- (void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler {
    NSURL *url = navigationAction.request.URL;
    if ([url.scheme isEqualToString:URLScheme]) {
        [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        self.webView.hidden = YES;
        self.closeButton.hidden = YES;
        decisionHandler(WKNavigationActionPolicyCancel);
    } else {
        decisionHandler(WKNavigationActionPolicyAllow);
    }
}

@end

RGNWebViewPlugin *globalWebViewPlugin;

extern "C" {
    void _RGNWebViewPlugin_OpenURL(const char *url) {
        if (globalWebViewPlugin == nil) {
            globalWebViewPlugin = [[RGNWebViewPlugin alloc] init];
        }
        [globalWebViewPlugin openURL:[NSString stringWithUTF8String:url]];
    }
}
