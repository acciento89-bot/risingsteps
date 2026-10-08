#import <UIKit/UIKit.h>
extern "C" void RSHaptic(int kind) {
 dispatch_async(dispatch_get_main_queue(), ^{
  if(kind==2){UINotificationFeedbackGenerator *g=[[UINotificationFeedbackGenerator alloc] init];[g notificationOccurred:UINotificationFeedbackTypeSuccess];}
  else {UIImpactFeedbackGenerator *g=[[UIImpactFeedbackGenerator alloc] initWithStyle:kind==1?UIImpactFeedbackStyleMedium:UIImpactFeedbackStyleLight];[g impactOccurred];}
 });
}
