import argparse
import time
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from PIL import Image


parser = argparse.ArgumentParser()
parser.add_argument('--site', type=str, default='https://www.craiyon.com')
parser.add_argument('--prompt', type=str, default='cute owl watercolor painting')
args = parser.parse_args()

PATH = "C:\Program Files (x86)\chromedriver.exe"\

options = Options()
options.add_argument("--window-size=1280x1280")
options.add_argument("--disable-web-security")
options.add_argument("--disable-site-isolation-trials")
options.headless = True

driver = webdriver.Chrome(PATH, options=options)
driver.get(args.site)
#driver.implicitly_wait(5)
time.sleep(2)
#if (driver.find_element(By.CLASS_NAME, "css-1litn2c") is not None):
#   driver.find_element(By.CLASS_NAME, "css-1litn2c").click()

driver.find_element(By.CLASS_NAME, "css-47sehv").click()
prompt = driver.find_element(By.ID, "prompt")
prompt.send_keys(args.prompt)
prompt.send_keys(Keys.ENTER)
#driver.implicitly_wait(70)
time.sleep(70)

driver.get_screenshot_as_file("J:\\full.png")
im = Image.open("J:\\full.png")
 
# Setting the points for cropped image
# All the coordinates of box (x, y, w, h) are measured from the top left corner of the image.
# So the coordinates of the box will be (x, y, w+x, h+y).
left = 355
top = 348
right = 525
bottom = 518
 
im1 = im.crop((left, top, right, bottom))
im1.save("C:\\UnityTextToImage\\Assets\\Selenium\\Images\\Craiyon\\" + args.prompt + ".png")

driver.quit()
