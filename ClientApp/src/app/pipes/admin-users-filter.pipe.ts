import {PipeTransform, Pipe } from '@angular/core'

@Pipe({
    name: 'usersAdminPipe'
})

export class UsersAdminPipe implements PipeTransform{
  
      transform(items: User[], searchText: string, selectedCategory: number): User[] {
        if(!items) return [];
        searchText = searchText.toLowerCase();
            return items.filter( it => {
              return (it.eMail.toLowerCase().includes(searchText) || it.name.toLowerCase().includes(searchText) || !searchText) && (selectedCategory == -1 || selectedCategory == it.userType);
            });
       }
}