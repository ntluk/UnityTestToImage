import argparse
import time
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.common.keys import Keys
from PIL import Image


parser = argparse.ArgumentParser()
parser.add_argument('--site', type=str, default='https://stablediffusionweb.com/#demo')
parser.add_argument('--prompt', type=str, default='cute owl watercolor painting')
args = parser.parse_args()

PATH = "C:\Program Files (x86)\chromedriver.exe"
options = Options()
options.add_argument("--window-size=1280x1280")
options.add_argument("--disable-web-security")
options.add_argument("--disable-site-isolation-trials")
options.headless = True

driver = webdriver.Chrome(PATH, options=options)
driver.get(args.site)
# driver.implicitly_wait(5)
time.sleep(2)
driver.switch_to.frame('inneriframe')

prompt = driver.find_element(By.XPATH, '//*[@id="prompt-text-input"]/label/input')
prompt.send_keys(args.prompt)
prompt.send_keys(Keys.ENTER)
# driver.implicitly_wait(45)
time.sleep(45)

#driver.find_element(By.CLASS_NAME, "h-full w-full rounded-lg border border-gray-500 object-cover object-center hover:ring-4 hover:ring-orange-400").click()
driver.get_screenshot_as_file("J:\\full2.png")
im = Image.open("J:\\full2.png")
 
# Setting the points for cropped image
# All the coordinates of box (x, y, w, h) are measured from the top left corner of the image.
# So the coordinates of the box will be (x, y, w+x, h+y).
left = 295
top = 675
right = 625
bottom = 1010
 
# Cropped image of above dimension
# (It will not change original image)
im1 = im.crop((left, top, right, bottom))
im1.save("C:\\UnityTextToImage\\Assets\\Selenium\\Images\\StableDiffusion\\" + args.prompt + ".png")

driver.quit()
